using System;
using System.Collections.Generic;
using System.Text;

namespace MyDB.Mocks.MockModels
{
    public class MockDeleteEntities
    {
        public Guid dbId { get; set; }
        public Guid tableId { get; set; }
        public List<int> entities { get; set; }
    }
}
