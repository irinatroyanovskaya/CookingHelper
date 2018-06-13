using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelegramCookingHelper.Classes.Interfaces;

namespace TelegramCookingHelper.Classes
{
    public class Factory
    {
        private static Factory _instance;

        public static Factory Instance => _instance ?? (_instance = new Factory());

        private IRepository _repo;

        public IRepository GetRepository() => _repo ?? (_repo = new DatabaseRepository());
    }
}
