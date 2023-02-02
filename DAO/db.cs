using CarServiceApp.Model.DataSource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarServiceApp.Model.DAO
{
    class db
    {
        private static AutoserviceEntities entity;

        public static AutoserviceEntities GetContext()
        {
            if (entity == null) entity = new AutoserviceEntities();
            return entity;
        }
    }
}
