namespace openPERTypes
{
    public struct PartNumber
    {
        private decimal m_value;

        public PartNumber(int partNumber)
        {
            m_value = partNumber;
        }
        public PartNumber(decimal partNumber)
        {
            m_value = partNumber;
        }

        public static implicit operator PartNumber(int partNumber)
        {
            return new PartNumber(partNumber);
        }
        public static implicit operator int(PartNumber partNumber)
        {
            return (int)partNumber.m_value;
        }
        public static implicit operator decimal(PartNumber partNumber)
        {
            return partNumber.m_value;
        }
        public static implicit operator double(PartNumber partNumber)
        {
            return (double)partNumber.m_value;
        }

        public override string ToString()
        {
            return m_value.ToString("###########0");
        }
    }
}
