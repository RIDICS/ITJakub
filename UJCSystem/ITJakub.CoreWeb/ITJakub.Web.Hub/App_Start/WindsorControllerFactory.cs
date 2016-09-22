using System.Collections.Generic;
using Castle.MicroKernel;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Internal;

namespace ITJakub.Web.Hub.App_Start
{
    public class WindsorControllerFactory : DefaultControllerFactory
    {
        private readonly IKernel m_kernel;

        //public WindsorControllerFactory(IKernel kernel)
        //{
        //    m_kernel = kernel;
        //}

        //public override void ReleaseController(IController controller)
        //{
        //    m_kernel.ReleaseComponent(controller);
        //}

        //protected override IController GetControllerInstance(RequestContext requestContext, Type controllerType)
        //{
        //    if (controllerType == null)
        //    {
        //        throw new HttpException(404, string.Format("The controller for path '{0}' could not be found.", requestContext.HttpContext.Request.Path));
        //    }
        //    return (IController)m_kernel.Resolve(controllerType);
        //}

        public WindsorControllerFactory(IControllerActivator controllerActivator, IEnumerable<IControllerPropertyActivator> propertyActivators) : base(controllerActivator, propertyActivators)
        {
        }
    }
}