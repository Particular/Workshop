namespace Divergent.Shipping.Data
{
    public static class VolumeEstimator
    {
        private const double Height = 0.1d;
        private const double Width = 0.1d;
        private const double Depth = 0.1d;

        public static double Calculate(int numberOfProducts)
        {
            return Height*Width*Depth*numberOfProducts;
        }
    }
}
