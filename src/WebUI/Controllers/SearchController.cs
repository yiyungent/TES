﻿using Framework.HtmlHelpers;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Search;
using Lucene.Net.Store;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebUI.Infrastructure.Search;
using WebUI.Models.SearchVM;

namespace WebUI.Controllers
{
    public class SearchController : Controller
    {
        string indexPath = System.Configuration.ConfigurationManager.AppSettings["lucenedir"];
        private SearchDbContext db = new SearchDbContext();

        #region 首页
        public ActionResult Index(string txtSearch, bool? hidfIsOr, int id = 1)
        {
            PagedList<SearchResult> list = null;
            if (!string.IsNullOrEmpty(txtSearch))//如果点击的是查询按钮
            {
                //list = Search(txtSearch);
                list = (hidfIsOr == null || hidfIsOr.Value == false) ? AndSearch(txtSearch, id) : OrSearch(txtSearch, id);
            }
            var keyWords = db.SearchTotal.OrderByDescending(a => a.SearchCounts).Select(x => x.KeyWords).Skip(0).Take(6).ToList();
            ViewBag.KeyWords = keyWords;
            return View(list);
        }
        #endregion

        #region 与查询
        //与查询
        PagedList<SearchResult> AndSearch(String kw, int pageIndex, int pageSize = 4)
        {
            FSDirectory directory = FSDirectory.Open(new DirectoryInfo(indexPath), new NoLockFactory());
            IndexReader reader = IndexReader.Open(directory, true);
            IndexSearcher searcher = new IndexSearcher(reader);
            PhraseQuery query = new PhraseQuery();//查询条件
            PhraseQuery titleQuery = new PhraseQuery();//标题查询条件
            List<string> lstkw = LuceneHelper.PanGuSplitWord(kw);//对用户输入的搜索条件进行拆分。

            foreach (string word in lstkw)
            {
                query.Add(new Term("Content", word));//contains("Content",word)
                titleQuery.Add(new Term("Title", word));
            }
            query.SetSlop(100);//两个词的距离大于100（经验值）就不放入搜索结果，因为距离太远相关度就不高了

            BooleanQuery bq = new BooleanQuery();
            //Occur.Should 表示 Or , Must 表示 and 运算
            bq.Add(query, BooleanClause.Occur.SHOULD);
            bq.Add(titleQuery, BooleanClause.Occur.SHOULD);

            TopScoreDocCollector collector = TopScoreDocCollector.create(1000, true);//盛放查询结果的容器
            searcher.Search(bq, null, collector);//使用query这个查询条件进行搜索，搜索结果放入collector

            int recCount = collector.GetTotalHits();//总的结果条数
            ScoreDoc[] docs = collector.TopDocs((pageIndex - 1) * pageSize, pageIndex * pageSize).scoreDocs;//从查询结果中取出第m条到第n条的数据

            List<SearchResult> list = new List<SearchResult>();
            string msg = string.Empty;
            string title = string.Empty;

            for (int i = 0; i < docs.Length; i++)//遍历查询结果
            {
                int docId = docs[i].doc;//拿到文档的id，因为Document可能非常占内存（思考DataSet和DataReader的区别）
                //所以查询结果中只有id，具体内容需要二次查询
                Document doc = searcher.Doc(docId);//根据id查询内容。放进去的是Document，查出来的还是Document
                SearchResult result = new SearchResult();
                result.Id = Convert.ToInt32(doc.Get("Id"));
                msg = doc.Get("Content");//只有 Field.Store.YES的字段才能用Get查出来
                result.Description = LuceneHelper.CreateHightLight(kw, msg);//将搜索的关键字高亮显示。
                title = doc.Get("Title");
                foreach (string word in lstkw)
                {
                    title = title.Replace(word, "<span style='color:red;'>" + word + "</span>");
                }
                //result.Title=LuceneHelper.CreateHightLight(kw, title);
                result.Title = title;
                result.CreateTime = Convert.ToDateTime(doc.Get("CreateTime"));
                result.Url = "/Article/Details?Id=" + result.Id + "&kw=" + kw;
                list.Add(result);
            }
            //先将搜索的词插入到明细表。
            SearchDetail _SearchDetail = new SearchDetail { Id = Guid.NewGuid(), KeyWords = kw, SearchDateTime = DateTime.Now };
            db.SearchDetail.Add(_SearchDetail);
            int r = db.SaveChanges();

            PagedList<SearchResult> lst = new PagedList<SearchResult>(list, pageIndex, pageSize, recCount);

            return lst;
        }
        #endregion

        #region 或查询
        //或查询
        PagedList<SearchResult> OrSearch(String kw, int pageNo, int pageLen = 4)
        {
            FSDirectory directory = FSDirectory.Open(new DirectoryInfo(indexPath), new NoLockFactory());
            IndexReader reader = IndexReader.Open(directory, true);
            IndexSearcher searcher = new IndexSearcher(reader);
            List<PhraseQuery> lstQuery = new List<PhraseQuery>();
            List<string> lstkw = LuceneHelper.PanGuSplitWord(kw);//对用户输入的搜索条件进行拆分。

            foreach (string word in lstkw)
            {
                PhraseQuery query = new PhraseQuery();//查询条件
                query.SetSlop(100);//两个词的距离大于100（经验值）就不放入搜索结果，因为距离太远相关度就不高了
                query.Add(new Term("Content", word));//contains("Content",word)

                PhraseQuery titleQuery = new PhraseQuery();//查询条件
                titleQuery.Add(new Term("Title", word));

                lstQuery.Add(query);
                lstQuery.Add(titleQuery);
            }

            BooleanQuery bq = new BooleanQuery();
            foreach (var v in lstQuery)
            {
                //Occur.Should 表示 Or , Must 表示 and 运算
                bq.Add(v, BooleanClause.Occur.SHOULD);
            }
            TopScoreDocCollector collector = TopScoreDocCollector.create(1000, true);//盛放查询结果的容器
            searcher.Search(bq, null, collector);//使用query这个查询条件进行搜索，搜索结果放入collector

            int recCount = collector.GetTotalHits();//总的结果条数
            ScoreDoc[] docs = collector.TopDocs((pageNo - 1) * pageLen, pageNo * pageLen).scoreDocs;//从查询结果中取出第m条到第n条的数据

            List<SearchResult> list = new List<SearchResult>();
            string msg = string.Empty;
            string title = string.Empty;

            for (int i = 0; i < docs.Length; i++)//遍历查询结果
            {
                int docId = docs[i].doc;//拿到文档的id，因为Document可能非常占内存（思考DataSet和DataReader的区别）
                //所以查询结果中只有id，具体内容需要二次查询
                Document doc = searcher.Doc(docId);//根据id查询内容。放进去的是Document，查出来的还是Document
                SearchResult result = new SearchResult();
                result.Id = Convert.ToInt32(doc.Get("Id"));
                msg = doc.Get("Content");//只有 Field.Store.YES的字段才能用Get查出来
                title = doc.Get("Title");
                //将搜索的关键字高亮显示。
                foreach (string word in lstkw)
                {
                    title = title.Replace(word, "<span style='color:red;'>" + word + "</span>");
                }
                result.Description = LuceneHelper.CreateHightLight(kw, msg);
                result.Title = title;
                result.CreateTime = Convert.ToDateTime(doc.Get("CreateTime"));
                result.Url = "/Article/Details?Id=" + result.Id + "&kw=" + kw;
                list.Add(result);
            }
            //先将搜索的词插入到明细表。
            SearchDetail _SearchDetail = new SearchDetail { Id = Guid.NewGuid(), KeyWords = kw, SearchDateTime = DateTime.Now };
            db.SearchDetail.Add(_SearchDetail);
            int r = db.SaveChanges();

            PagedList<SearchResult> lst = new PagedList<SearchResult>(list, pageNo, pageLen, recCount);

            return lst;
        }
        #endregion
    }
}