using BaseController;
using Microsoft.AspNetCore.Mvc;
using MyDB.Application.CRUD.Models.Database;
using MyDB.Application.CRUD.Models.Database.Payloads;
using MyDB.Application.CRUD.Models.Database.Responses;
using MyDB.Domain.CRUD.DatabaseService;
using MyDB.Infrastructure.BaseController.Applications;
using MyDB.Infrastructure.BaseController.Model;
using System;
using System.Collections.Generic;
using System.Text.Json;

namespace CRUD
{
    public class QueryController : CRUDBaseController
    {
        #region Constructor/Attributes
        public QueryController(IServiceProvider serviceProvider) : base(serviceProvider){ }
        #endregion

        #region Routes
        /// <summary>
        /// Generate list DB off all databases on cache
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult<APIResponse<List<Guid>>> listDBs()
        {
            return base.BeginCommonHandler<List<Guid>>(response => {
                response.content = _mapper.Map<List<Guid>>(_databaseService.getListDB());
            });
        }

        /// <summary>
        /// Generate dump from database, using id received
        /// </summary>
        /// <param name="dbId"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult<APIResponse<FullDatabaseViewModel>> dump(Guid dbId)
        {
            return base.BeginCommonHandler<FullDatabaseViewModel>(response => {
                response.content = _mapper.Map<FullDatabaseViewModel>(_databaseService.getDB(dbId));
            });
        }

        /// <summary>
        /// Generate a full data from database, using id received. All references will be replaced
        /// </summary>
        /// <param name="dbId"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult<APIResponse<FullDatabaseViewModel>> fullDb(Guid dbId)
        {
            return base.BeginCommonHandler<FullDatabaseViewModel>(response => {
                response.content = _mapper.Map<FullDatabaseViewModel>(_databaseService.getDB(dbId, true));
            });
        }

        [HttpGet]
        public ActionResult<APIResponse<TableViewModel>> fullTable(Guid tableId)
        {
            return base.BeginCommonHandler<TableViewModel>(response => {
                response.content = _mapper.Map<TableViewModel>(_databaseService.getTable(tableId, true));
            });
        }

        #endregion
    }
}
