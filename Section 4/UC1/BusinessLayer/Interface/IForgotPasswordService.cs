using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModelLayer.DTO;

namespace BusinessLayer.Interface
{
    public interface IForgotPasswordService
    {
        Task<bool> ForgotPasswordAsync(ForgotPasswordDTO dto);
        Task<bool> ResetPasswordAsync(ResetPasswordDTO dto);
    }
}
