using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLayer.Interface;
using RepositoryLayer.Interface;

namespace BusinessLayer.Service
{
    public class AddBL : IAddBL
    {
        private readonly IAddRL _addRL;
        public AddBL(IAddRL addRL)
        {
            _addRL = addRL;
        }
    }
}
