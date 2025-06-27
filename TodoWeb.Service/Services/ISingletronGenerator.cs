using Microsoft.Extensions.DependencyInjection;

namespace TodoWeb.Application.Services
{
    public interface ISingletronGenerator
    {
        Guid Generator();
    }

    public class SingletronGenerator : ISingletronGenerator
    {
        //private readonly IGuidGenerator _guidGenerator;
        private readonly IServiceProvider _serviceProvider;
        public SingletronGenerator(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        public Guid Generator()
        {
            var guidGenerator = _serviceProvider.GetService<IGuidGenerator>();
            return guidGenerator.Generate();
        }
    }
}
