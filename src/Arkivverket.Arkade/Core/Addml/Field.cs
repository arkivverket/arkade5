namespace Arkivverket.Arkade.Core.Addml
{
    public class Field
    {
        // todo - add FieldDefinition

        public string Value { private set; get; }

        public Field(string value)
        {
            Value = value;
        }
    }
}