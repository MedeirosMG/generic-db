using MyDB.Application.CRUD.Models.Database.Payloads;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace MyDB.Mocks
{
    public static class GenericMock
    {
        public static IEnumerable<object[]> getMock<T>(string dir)
        {
            var data = File.ReadAllText($"ConfigsMock\\{dir}.json");
            var parsedData = JsonSerializer.Deserialize<List<T>>(data);

            return new List<object[]> { parsedData.Cast<object>().ToArray() };
        }
    }
}
