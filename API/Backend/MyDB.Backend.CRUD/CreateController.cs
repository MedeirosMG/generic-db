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
    public class CreateController : CRUDBaseController
    {
        #region Constructor/Attributes
        public CreateController(IServiceProvider serviceProvider) : base(serviceProvider){ }
        #endregion

        #region Routes
        /// <summary>
        /// Create a new database
        /// </summary>
        /// <param name="dbName"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult<APIResponse<DatabaseViewModel>> db(string dbName)
        {
            return base.BeginCommonHandler<DatabaseViewModel>(response => {
                response.content = _mapper.Map<DatabaseViewModel>(_databaseService.createDB(dbName));
            });
        }

        /// <summary>
        /// Create a new table for requested database
        /// </summary>
        /// <param name="payload"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult<APIResponse<TableViewModel>> table(DataTablePayload payload)
        {
            return base.BeginCommonHandler<TableViewModel>(response => {
                response.content = _mapper.Map<TableViewModel>(_databaseService.createTable(payload.databaseKey, payload.tableName
                    , _mapper.Map<List<TableAttribute>>(payload.attributes)));
            });
        }

        /// <summary>
        /// Create a new entity for requested table
        /// </summary>
        /// <param name="payload"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult<APIResponse<TableViewModel>> entity(DataEntityPayload payload)
        {            
            return base.BeginCommonHandler<TableViewModel>(response => {
                response.content = _mapper.Map<TableViewModel>(_databaseService.addEntity(payload.databaseKey, payload.tableKey, payload.entity));
            });
        }
        #endregion
    }
}
