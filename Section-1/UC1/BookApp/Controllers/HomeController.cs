using BusinessLayer.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;

namespace BookApp.Controllers
{
    [TestFixture]
    [Route("api/[controller")]
    [ApiController]
    public class HomeController : Controller
    {
        public IAddBL _addBL;
        public HomeController(IAddBL addBL)
        {
            _addBL = addBL;
        }
    }
}
