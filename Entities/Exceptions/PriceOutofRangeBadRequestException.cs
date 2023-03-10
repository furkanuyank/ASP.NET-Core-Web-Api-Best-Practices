namespace Entities.Exceptions
{
    public abstract partial class BadRequestException
    {
        public class InvalidPriceFiltersBadRequestException : BadRequestException
        {
            public InvalidPriceFiltersBadRequestException():base("Maximum price should be greater than minimum price.")
            {
            }
        }
    }
}
