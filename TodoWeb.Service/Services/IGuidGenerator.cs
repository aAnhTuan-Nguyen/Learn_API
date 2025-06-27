namespace TodoWeb.Application.Services
{
    public interface IGuidGenerator
    {
        Guid Generate();
    }

    public class GuidGenerator : IGuidGenerator
    {
        Guid guid;

        public GuidGenerator()
        {
            guid = Guid.NewGuid();
        }
        public Guid Generate()
        {
            return guid;
        }


    }
}
