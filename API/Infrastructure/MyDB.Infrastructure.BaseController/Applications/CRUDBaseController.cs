using BaseController;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyDB.Infrastructure.BaseController.Applications
{
    [Route("Database/[controller]/[action]")]
    public abstract class CRUDBaseController : BaseControllerApplication
    {
        public CRUDBaseController(IServiceProvider serviceProvider) : base(serviceProvider) { }
    }
}
