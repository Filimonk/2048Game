namespace _Scripts
{
    public static class PowForInt
    {
        public static int PowInt(int n, int a)
        {
            if (n == 0)
            {
                return 1;
            }

            int halfPowerValue = PowInt(n / 2, a);
            if (n % 2 == 1)
            {
                return halfPowerValue * halfPowerValue * a;
            }
            else
            {
                return halfPowerValue * halfPowerValue;
            }
        }
    }
}