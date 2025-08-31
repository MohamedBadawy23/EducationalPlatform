using Core.Entities;
using CoreProject.Enteties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Specefications;

namespace CoreProject.Specefications
{
   public class CenterWitheUserSpecefication:Specefication<Center>
    {
        public CenterWitheUserSpecefication()
        {
            //AddInclude(c => c.User); // لو حابب تجيب بيانات المستخدم المرتبط بالسنتر
            //AddInclude(c => c.Teachers); // لو حابب تجيب المدرسين
            //AddInclude(c => c.Students); // لو حابب تجيب الطلبة
            //AddInclude(c => c.Subjects);  // لو حابب تجيب المواد
        }
    }
}
