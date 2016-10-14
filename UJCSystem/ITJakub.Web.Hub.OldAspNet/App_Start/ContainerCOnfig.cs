namespace ITJakub.Web.Hub
{
    public class ContainerConfig
    {
        public static void InitializeContainers()
        {
            var container = Container.Current;
        }

        public static void CleanUpContainers()
        {
            var container = Container.Current;
            container.Dispose();
        }
    }
}