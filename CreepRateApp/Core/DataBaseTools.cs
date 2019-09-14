using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MongoDB.Driver;
using MongoDB.Bson;
using MongoDB.Driver.Builders;
using System.Configuration;
using System.IO;
using System.Windows.Forms;

namespace CreepRateApp.Core
{
    public static class DataBaseTools
    {
        private static string ConnectionString = GlobalValue.DbConnString;
        private static string DBName = GlobalValue.DbName;

        /// <summary>
        /// 获取数据库实例对象
        /// </summary>
        /// <param name="connectionString">数据库连接字符串</param>
        /// <param name="dbName">数据库名称</param>
        /// <returns>数据库实例对象</returns>
        public static MongoDatabase GetDatabase()
        {
            MongoClient server = null;
            try
            {
                server = new MongoClient(ConnectionString);
            }
            catch
            {
                server = null;
            }

            if (server != null)
            {
                int mDataBaseNamesCount = 0;
                try
                {
                    mDataBaseNamesCount = server.GetServer().GetDatabaseNames().Count();
                }
                catch
                { 
                
                }
                if (mDataBaseNamesCount > 0)
                {
                    return server.GetServer().GetDatabase(DBName);
                }
                else
                {
                    return null;
                }
            }
            else
                return null;
            
        }

        /// <summary>
        /// 插入一条记录
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="connectionString">数据库连接字符串</param>
        /// <param name="dbName">数据名称</param>
        /// <param name="collectionName">集合名称</param>
        /// <param name="model">数据对象</param>
        public static void Insert<T>(string collectionName, T model) where T : EntityBase
        {
            if (model == null)
            {
                throw new ArgumentNullException("model", "待插入数据不能为空");
            }
            var db = GetDatabase();
            if (db != null)
            {
                var collection = db.GetCollection<T>(collectionName);
                var WriteResult = collection.Insert(model);
            }
        }

        /// <summary>
        /// 更新数据
        /// </summary>
        /// <param name="connectionString">数据库连接字符串</param>
        /// <param name="dbName">数据库名称</param>
        /// <param name="collectionName">集合名称</param>
        /// <param name="query">查询条件</param>
        /// <param name="dictUpdate">更新字段</param>
        public static void Update<T>(string collectionName, IMongoQuery query, T t) where T:EntityBase
        {
            var db = GetDatabase();
            if (db != null)
            {
                var collection = db.GetCollection(collectionName);
                MongoCollection<T> col = db.GetCollection<T>(collectionName);
                BsonDocument bd = BsonExtensionMethods.ToBsonDocument(t);
                col.Update(query, new UpdateDocument(bd));
            }
        }

        /// <summary>
        /// 根据ID获取数据对象
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="connectionString">数据库连接字符串</param>
        /// <param name="dbName">数据库名称</param>
        /// <param name="collectionName">集合名称</param>
        /// <param name="id">ID</param>
        /// <returns>数据对象</returns>
        public static T GetById<T>(string collectionName, ObjectId id)
            where T : EntityBase
        {
            var db = GetDatabase();
            if (db != null)
            {
                var collection = db.GetCollection<T>(collectionName);
                return collection.FindOneById(id);
            }
            else
                return null;
        }

        /// <summary>
        /// 根据查询条件获取一条数据
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="connectionString">数据库连接字符串</param>
        /// <param name="dbName">数据库名称</param>
        /// <param name="collectionName">集合名称</param>
        /// <param name="query">查询条件</param>
        /// <returns>数据对象</returns>
        public static T GetOneByCondition<T>(string collectionName, IMongoQuery query)
            where T : EntityBase
        {
            var db = GetDatabase();
            if (db != null)
            {
                var collection = db.GetCollection<T>(collectionName);
                return collection.FindOne(query);
            }
            else
                return null;
        }

        /// <summary>
        /// 根据查询条件获取多条数据
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="connectionString">数据库连接字符串</param>
        /// <param name="dbName">数据库名称</param>
        /// <param name="collectionName">集合名称</param>
        /// <param name="query">查询条件</param>
        /// <returns>数据对象集合</returns>
        public static List<T> GetManyByCondition<T>(string collectionName, IMongoQuery query)
            where T : EntityBase
        {
            var db = GetDatabase();
            if (db != null)
            {
                var collection = db.GetCollection<T>(collectionName);
                return collection.Find(query).ToList();
            }
            else
                return null;
        }

        public static List<T> GetPageQueryByCondition<T>(String collectionName, IMongoQuery query, int limit, int skip)
        {
            var db = GetDatabase();
            if (db != null)
            {
                var collection = db.GetCollection<T>(collectionName);
                var mongoCursor = collection.FindAs<T>(query);
                mongoCursor.SetSkip(skip);
                mongoCursor.SetLimit(limit);
                return mongoCursor.ToList();
            }
            else
                return null;
        }

        /// <summary>
        /// 根据集合中的所有数据
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="connectionString">数据库连接字符串</param>
        /// <param name="dbName">数据库名称</param>
        /// <param name="collectionName">集合名称</param>
        /// <returns>数据对象集合</returns>
        public static List<T> GetAll<T>(string collectionName)
            where T : EntityBase
        {
            var db = GetDatabase();
            if (db != null)
            {
                var collection = db.GetCollection<T>(collectionName);
                return collection.FindAll().ToList();
            }
            else
                return null;
        }

        /// <summary>
        /// 删除集合中符合条件的数据
        /// </summary>
        /// <param name="connectionString">数据库连接字符串</param>
        /// <param name="dbName">数据库名称</param>
        /// <param name="collectionName">集合名称</param>
        /// <param name="query">查询条件</param>
        public static void DeleteByCondition(string collectionName, IMongoQuery query)
        {
            var db = GetDatabase();
            if (db != null)
            {
                var collection = db.GetCollection(collectionName);
                collection.Remove(query);
            }
        }

        /// <summary>
        /// 删除集合中的所有数据
        /// </summary>
        /// <param name="connectionString">数据库连接字符串</param>
        /// <param name="dbName">数据库名称</param>
        /// <param name="collectionName">集合名称</param>
        public static void DeleteAll(string collectionName)
        {
            var db = GetDatabase();
            if (db != null)
            {
                var collection = db.GetCollection(collectionName);
                collection.RemoveAll();
            }
        }

        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="filePath">文件本地路径</param>
        /// <returns></returns>
        public static MongoDB.Driver.GridFS.MongoGridFSFileInfo UpFile(string filePath)
        {
            var db = GetDatabase();
            if (db != null)
            {
                var fs = new FileStream(filePath, FileMode.Open);
                var fileName = System.IO.Path.GetFileName(filePath);
                var gridFsInfo = db.GridFS.Upload(fs, fileName);
                var fileId = gridFsInfo.Id;
                fs.Close();
                return gridFsInfo;
            }
            else
                return null;
        }

        /// <summary>
        /// 读取文件
        /// </summary>
        /// <param name="fileId">文件id</param>
        /// <returns></returns>
        public static MongoDB.Driver.GridFS.MongoGridFSFileInfo ReadFile(string fileId)
        {
            var db = GetDatabase();
            if (db != null)
            {
                ObjectId oid = new ObjectId(fileId);
                return db.GridFS.FindOneById(oid);
            }
            else
                return null;
        }

        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="fileId">文件id</param>
        public static void DeleteFile(string fileId)
        {
            var db = GetDatabase();
            if (db != null)
            {
                ObjectId oid = new ObjectId(fileId);
                db.GridFS.DeleteById(oid);
            }
        }
    }
}
