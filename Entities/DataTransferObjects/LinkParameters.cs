using Entities.RequestFeatures;
using Microsoft.AspNetCore.Http;

namespace Entities.DataTransferObjects
{
    public record LinkParameters
    {
        public BookParameters BookParameters { get; init; }
        public HttpContext HtppContext { get; init; }

    }
}
