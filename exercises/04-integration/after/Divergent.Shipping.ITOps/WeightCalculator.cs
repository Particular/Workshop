namespace Divergent.Shipping.ITOps
{
    public static class WeightCalculator
    {
        private const double WeightPerProduct = 0.6d;

        public static double CalculateWeight(int numberOfProducts)
        {
            return numberOfProducts * WeightPerProduct;
        }
    }
}
