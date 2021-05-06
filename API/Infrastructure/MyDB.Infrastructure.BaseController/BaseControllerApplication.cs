using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using MyDB.Application.CRUD.DatabaseService.Exceptions;
using MyDB.Application.CRUD.DatabaseService.Interfaces;
using MyDB.Infrastructure.BaseController.Model;
using MyDB.Infrastructure.Cache.Interfaces;
using MyDB.Infrastructure.Tools.Interfaces;
using System;

namespace BaseController
{
    [ApiController]
    [Consumes("application/json")]
    public class BaseControllerApplication : ControllerBase
    {
        #region Constructor/Attributes
        protected IUtilityService _utilityService { get; set; }
        protected ICacheService _redisService { get; set; }
        protected IDatabaseService _databaseService { get; set; }
        protected IMapper _mapper { get; set; }
        public BaseControllerApplication(IServiceProvider serviceProvider) 
        {
            _redisService = serviceProvider.GetRequiredService<ICacheService>();
            _utilityService = serviceProvider.GetRequiredService<IUtilityService>();
            _databaseService = serviceProvider.GetRequiredService<IDatabaseService>();
            _mapper = serviceProvider.GetRequiredService<IMapper>();
        }
        #endregion

        protected ActionResult<APIResponse<T>> BeginCommonHandler<T>(Action<APIResponse<T>> func)
        {
            APIResponse<T> response = new APIResponse<T>() { message = "" };
            if (!_utilityService.checkPrimitive(typeof(T)))
                response.content = Activator.CreateInstance<T>();

            try
            {
                func.Invoke(response);
            } catch (DatabaseNotFoundException ex)
            {
                // TODO: Log service

                response.message = ex.Message;
                this.HttpContext.Response.StatusCode = 404;
            } catch (Exception ex)
            {
                // TODO: Log service

                response.message = ex.Message;
                this.HttpContext.Response.StatusCode = 500;
            }

            return response;
        }
    }
}
