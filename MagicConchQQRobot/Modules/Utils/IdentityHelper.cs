using MagicConchQQRobot.DataObjs.DbClass;
using MagicConchQQRobot.Globals;
using System.Text.RegularExpressions;

namespace MagicConchQQRobot.Modules.Utils
{
    class IdentityHelper
    {
        public static User GetUserObj(long userId)
        {
            User userItem = User.Find(User._.Uid == userId);
            if (userItem != null)
            {
                return userItem;
            }
            return null;
        }

        public static bool IsRobotUser(long userId)
        {
            User userItem = User.Find(User._.Uid == userId);
            if (userItem != null && userItem.Level > 0 || GlobalSet.IsAnonymousAllowed)
            {
                return true;
            }
            return false;
        }

        public static bool IsRobotUser(User userObj)
        {
            if (userObj != null && userObj.Level > 0 || GlobalSet.IsAnonymousAllowed)
            {
                return true;
            }
            return false;
        }

        public static bool IsAdmin(long userId)
        {
            UserAdmin userItem = UserAdmin.Find(UserAdmin._.Uid == userId);
            if (userItem != null)
            {
                if (userItem.Uid == userId && userItem.AdminLevel > 0)
                {
                    return true;
                }
            }
            return false;
        }

        public static bool IsSuperNoobMember(long userId)
        {
            Member memberItem = Member.Find(Member._.qq == userId);
            return memberItem != null;
        }

        public static bool IsUserBanned(long userId)
        {
            User userItem = User.Find(User._.Uid == userId);
            if (userItem != null)
            {
                return userItem.Banned == 1;
            }
            return false;
        }

        public static bool IsUserBanned(User userObj)
        {
            if (userObj != null)
            {
                return userObj.Banned == 1;
            }
            return false;
        }

        public static int GetUserAccessLevel(long userId)
        {
            User userItem = User.Find(User._.Uid == userId);
            if (userItem != null)
            {
                return userItem.Level;
            }
            return 0;
        }

        public static int GetUserAccessLevel(User userObj)
        {
            if (userObj != null)
            {
                return userObj.Level;
            }
            return 0;
        }

        public static User UserRegister(long userUid, string nickname, int level = 0)
        {
            User user = new User
            {
                Uid = userUid,
                Nickname = Base64Helper.Encode(nickname),
                Level = level,
                Regtime = TimestampHelper.GetTimestampSecond()
            };
            user.Insert();
            return user;
        }

        public static string GetUserSuperNoobId(string checkString)
        {
            Regex rgx = new Regex(@"(?<=\u3010)\S+?(?=\u3011)");
            Match match = rgx.Match(checkString);

            if (match.Success)
            {
                return match.Groups[0].Value;
            }
            else
            {
                return null;
            }

        }
    }
}
