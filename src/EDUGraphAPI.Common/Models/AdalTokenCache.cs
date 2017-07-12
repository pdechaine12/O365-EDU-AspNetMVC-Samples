using EDUGraphAPI.Data;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using System.Linq;
using System.Web.Security;

namespace EDUGraphAPI.Models
{
    public class AdalTokenCache : TokenCache
    {
        private const string MachineKeyProtectPurpose = "ADALCache";

        private readonly string userId;

        public AdalTokenCache(string signedInUserId)
        {
            this.userId = signedInUserId;
            this.AfterAccess = AfterAccessNotification;
            this.BeforeAccess = BeforeAccessNotification;

            GetCacheAndDeserialize();
        }

        public override void Clear()
        {
            base.Clear();
            ClearUserTokenCache(userId);
        }

        void BeforeAccessNotification(TokenCacheNotificationArgs args)
        {
            GetCacheAndDeserialize();
        }

        void AfterAccessNotification(TokenCacheNotificationArgs args)
        {
            if (this.HasStateChanged)
            {
                SerializeAndUpdateCache();
                this.HasStateChanged = false;
            }
        }


        private void GetCacheAndDeserialize()
        {
            var cacheBits = GetUserTokenCache(userId);
            if (cacheBits != null)
            {
                try
                {
                    var data = MachineKey.Unprotect(cacheBits, MachineKeyProtectPurpose);
                    this.Deserialize(data);
                }
                catch { }
            }
        }

        private void SerializeAndUpdateCache()
        {
            var cacheBits = MachineKey.Protect(this.Serialize(), MachineKeyProtectPurpose);
            UpdateUserTokenCache(userId, cacheBits);
        }


        private byte[] GetUserTokenCache(string userId)
        {
            using (var db = new ApplicationDbContext())
            {
                var cache = GetUserTokenCache(db, userId);
                return cache?.cacheBits;
            }
        }

        private static void UpdateUserTokenCache(string userId, byte[] cacheBits)
        {
            using (var db = new ApplicationDbContext())
            {
                var cache = GetUserTokenCache(db, userId);
                if (cache == null)
                {
                    cache = new UserTokenCache { webUserUniqueId = userId };
                    db.UserTokenCacheList.Add(cache);
                }

                cache.cacheBits = cacheBits;
                cache.LastWrite = DateTime.UtcNow;

                db.SaveChanges();
            }
        }

        private static UserTokenCache GetUserTokenCache(ApplicationDbContext db, string userId)
        {
            return db.UserTokenCacheList
                   .OrderByDescending(i => i.LastWrite)
                   .FirstOrDefault(c => c.webUserUniqueId == userId);
        }

        private static void ClearUserTokenCache(string userId)
        {
            using (var db = new ApplicationDbContext())
            {
                var cacheEntries = db.UserTokenCacheList
                    .Where(c => c.webUserUniqueId == userId)
                    .ToArray();
                db.UserTokenCacheList.RemoveRange(cacheEntries);
                db.SaveChanges();
            }
        }
    }
}
