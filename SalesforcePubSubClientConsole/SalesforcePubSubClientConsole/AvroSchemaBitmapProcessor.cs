using BitArray = System.Collections.BitArray;
using Avro;
using Field = Avro.Field;
using Schema = Avro.Schema;

namespace ConsoleApp4
{

    public class AvroSchemaBitmapProcessor
    {
        public List<string> ProcessBitmap(RecordSchema avroSchema, List<string> bitmapFields)
        {
            var fields = new List<string>();

            if (bitmapFields.Count != 0)
            {
                if (bitmapFields[0].StartsWith("0x"))
                {
                    string bitmap = bitmapFields[0];
                    fields.AddRange(GetFieldnamesFromBitstring(bitmap, avroSchema));
                    bitmapFields.Remove(bitmap);
                }

                if (bitmapFields.Count != 0 && bitmapFields.Any(bf => bf != null && bf.Contains("-")))
                {
                    foreach (string bitmapField in bitmapFields)
                    {
                        if (bitmapField != null && bitmapField.Contains("-"))
                        {
                            string[] bitmapStrings = bitmapField.Split('-');
                            Field parentField = avroSchema.Fields[int.Parse(bitmapStrings[0])];
                            Schema childSchema = GetValueSchema(parentField);

                            if (childSchema.Tag != null && childSchema.Tag == Schema.Type.Record)
                            {
                                var childRecordSchema = (RecordSchema)childSchema;
                                int nestedSize = childRecordSchema.Fields.Count;
                                string parentFieldName = parentField.Name;
                                List<string> fullFieldNames = GetFieldnamesFromBitstring(bitmapStrings[1], childRecordSchema);
                                fullFieldNames = AppendParentName(parentFieldName, fullFieldNames);

                                if (fullFieldNames != null && fullFieldNames.Count > 0)
                                {
                                    fields.AddRange(fullFieldNames);
                                }
                            }
                        }
                    }
                }
            }

            return fields;
        }

        private string ConvertHexBinaryToBitset(string bitmap)
        {
            try
            {
                int number = Convert.ToInt32(bitmap, 16);
                var bitArray = new BitArray(new[] { number });
                string binaryString = string.Join("", bitArray.Cast<bool>().Select(bit => bit ? '1' : '0'));
                return new string(binaryString.ToArray());

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error converting Hex Binary to Bitset. {ex.Message}");
                return null;
            }   
        }

        private List<string> AppendParentName(string parentFieldName, List<string> fullFieldNames)
        {
            if (fullFieldNames != null && fullFieldNames.Count > 0)
            {
                for (int index = 0; index < fullFieldNames.Count; index++)
                {
                    fullFieldNames[index] = parentFieldName + "." + fullFieldNames[index];
                }
            }

            return fullFieldNames;
        }

        private List<string> GetFieldnamesFromBitstring(string bitmap, RecordSchema avroSchema)
        {
            try
            {
                var bitmapFieldNames = new List<string>();
                IList<Field> fieldsList = avroSchema.Fields.OrderBy(o => o.Pos).ToList();
                string binaryString = ConvertHexBinaryToBitset(bitmap);
                List<int> indexes = Find('1', binaryString);

                foreach (int index in indexes)
                {
                    var field = fieldsList[index];
                    var valueSchema = GetValueSchema(field);
                    bitmapFieldNames.Add(field.Name);
                }

                return bitmapFieldNames;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting field names from bitstring. {ex.Message}");
                return new List<string>();
            }
        }

        private Schema GetValueSchema(Field parentField)
        {
            if (parentField.Schema.Tag == Schema.Type.Union)
            {
                var schemas = (UnionSchema)parentField.Schema;
                if (schemas.Count == 2 && schemas.Schemas[0].Tag == Schema.Type.Null)
                {
                    return schemas[1];
                }
                if (schemas.Count == 2 && schemas.Schemas[0].Tag == Schema.Type.String)
                {
                    return schemas[1];
                }
                if (schemas.Count == 3 && schemas.Schemas[0].Tag == Schema.Type.Null && schemas.Schemas[1].Tag == Schema.Type.String)
                {
                    return schemas[2];
                }
            }

            return parentField.Schema;
        }

        private List<int> Find(char toFind, string binaryString)
        {
            return binaryString.Select((value, index) => (value, index))
                               .Where(pair => pair.value == toFind)
                               .Select(pair => pair.index)
                               .ToList();
        }
    }
}

