using Dapper;
using Kogel.Dapper.Extension.MsSql;
using Sxb.Framework.Foundation;
using Sxb.User.Common.DTO;
using Sxb.User.Common.Entity;
using Sxb.User.Query.SQL.DB;
using Sxb.User.Query.SQL.IRepository;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.User.Query.SQL.Repository
{
    public class TalentRepository : ITalentRepository
    {
        SchoolUserDB _userDB;
        public TalentRepository(SchoolUserDB schoolUserDB)
        {
            _userDB = schoolUserDB;
        }

        public async Task<TalentDTO> GetByUserID(Guid userID)
        {
            if (userID == default) return null;
            var str_SQL = @"
				select
					top 1
					u.id as [ID],
					u.nickname as [Nickname],
					u.HeadImgUrl as [HeadImgUrl],
					t.type as [Role],
					u.introduction as [Introduction],
					t.certification_title as [Certification_title],
					t.certification_preview as [Certification_preview],
					e.eid as [EID],
					t.organization_name as [Organization_name],
					case when t.certification_status = 1 and t.isdelete = 0 and t.status = 1 then 1 else 0 end as [IsAuth],
					case when ts.talent_id is not null then 1 else 0 end as [IsTalentStaff],
					pt.user_id as [ParentUserId]
				from  userInfo as u 
					left join talent as t on t.user_id = u.id and t.isdelete = 0
					left join talent_staff as ts on ts.talent_id= t.id and ts.isdelete= 0
					left join talent as pt on pt.id = ts.parentId and pt.isdelete = 0
					left join talentSchoolExtension as e on t.id = e.talentId
					where u.id = @userID";
            return await _userDB.SlaveConnection.QuerySingleAsync<TalentDTO>(str_SQL, new { userID });
        }

        public async Task<Guid> GetExtensionTalentUserID(Guid eid)
        {
            if (eid == default) return default;

            var str_SQL = $@"SELECT
	                            u.id
                            FROM
	                            userInfo AS u
	                            LEFT JOIN talent AS t ON t.user_id = u.id
	                            LEFT JOIN talentSchoolExtension AS tse ON tse.talentId = t.id 
                            WHERE
                                t.isdelete = 0 AND
	                            tse.eid = @eid";
            var finds = await _userDB.SlaveConnection.QueryAsync<Guid>(str_SQL, new { eid });
            //var finds = await _userDB.QuerySet<TalentDTO>().Join<TalentDTO, TalentSchoolExtensionInfo>((x, y) => x.ID == y.TalentID).Where<TalentSchoolExtensionInfo>(p => p.EID == eid).Where(p => p.IsDelete == false).ToListAsync(p => p.UserID);
            if (finds?.Any() == true)
            {
                return finds.ToArray()[new Random().Next(0, finds.Count())];
            }
            return default;
        }

        public async Task<IEnumerable<TalentDTO>> ListByUserIDs(IEnumerable<Guid> userIDs)
        {
            if (userIDs?.Any() == true)
            {
                var userInfos = await _userDB.SlaveConnection.QuerySet<UserInfo>().Where($"ID In ('{string.Join("','", userIDs.Distinct())}')").ToIEnumerableAsync();
                if (userInfos?.Any() == true)
                {
                    var dtos = userInfos.Select(p =>
                    {
                        return CommonHelper.MapperProperty<UserInfo, TalentDTO>(p, true);
                    });
                    var talents = await _userDB.SlaveConnection.QuerySet<TalentInfo>().Where($"User_ID in ('{string.Join("','", userInfos.Select(p => p.ID))}')").ToIEnumerableAsync();
                    if (talents?.Any() == true)
                    {
                        //var tsesTasks = _userDB.QuerySet<TalentSchoolExtensionInfo>().Where(p => p.TalentID.In(talents.Select(p => p.ID).ToArray())).ToIEnumerableAsync();
                        var tsesTasks = _userDB.SlaveConnection.QuerySet<TalentSchoolExtensionInfo>().Where($"TalentID In ('{string.Join("','", talents.Select(p => p.ID))}')").ToIEnumerableAsync();
                        await Task.Delay(20);
                        //var tssTask = _userDB.QuerySet<TalentStaffInfo>().Where(p => p.Talent_ID.In(talents.Select(p => p.ID).ToArray())).ToIEnumerableAsync();
                        var tssTask = _userDB.SlaveConnection.QuerySet<TalentStaffInfo>().Where($"Talent_ID In ('{string.Join("','", talents.Select(p => p.ID))}')").ToIEnumerableAsync();
                        await Task.WhenAll(tsesTasks, tssTask);
                        var tses = tsesTasks.Result;
                        var tss = tssTask.Result;
                        foreach (var dto in dtos)
                        {
                            var talent = talents.FirstOrDefault(p => p.UserID == dto.ID);
                            if (talent != null)
                            {
                                dto.Role = talent.Type;
                                dto.Certification_title = talent.Certification_Title;
                                dto.Certification_preview = talent.Certification_Preview;
                                dto.Organization_name = talent.Organization_Name;
                                if (talent.Certification_Status == 1 && !talent.IsDelete && talent.Status == 1) dto.IsAuth = true;
                                if (tss.Any(p => p.Talent_ID == talent.ID)) dto.ParentUserId = tss.FirstOrDefault(p => p.Talent_ID == talent.ID)?.ParentID;
                                if (tses.Any(p => p.TalentID == talent.ID)) dto.EID = tses.FirstOrDefault(p => p.TalentID == talent.ID).EID;
                            }

                        }
                    }
                    return dtos;
                }
            }
            return null;
        }
    }
}
