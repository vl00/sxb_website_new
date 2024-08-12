using MediatR;
using Sxb.School.API.Application.Models;
using Sxb.School.Domain.AggregateModels.DgAyOrderAggregate;
using System;
using System.Collections.Generic;

namespace Sxb.School.API.Application.Commands
{
    public class CreateDgAyOrderCommand:IRequest<DgAyOrder>
    {
        public Guid? UserId { get; set; }




        /// <summary>
        /// 终端类型 1->h5 2->pc 3->小程序
        /// </summary>
        public byte Termtyp { get; set; }

        public List<DgAyOrderProductInfo> ProductInfos { get; set; }


    }
}
