using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ai_tool_suite.Models;
using OpenAI_API;
using OpenAI_API.Completions;

namespace ai_tool_suite.Controllers
{
    public class PromptsController : Controller
    {
        private UserContext db = new UserContext();
        public string apiKey = "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";

        // GET: Prompts
        public async Task<ActionResult> Index()
        {
            return View(await db.Prompts.ToListAsync());
        }

        // GET: Prompts/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Prompts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "ID,Prompt,Result,PromptTokens,CompletionTokens,TotalTokens")] Prompts prompts)
        {
            if (ModelState.IsValid)
            {
                var encoding = Tiktoken.Encoding.ForModel("gpt-4");
                var tokens = encoding.Encode(prompts.Prompt);
                prompts.PromptTokens = encoding.CountTokens(prompts.Prompt);
                prompts.TotalTokens += prompts.PromptTokens; 
                db.Prompts.Add(prompts);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(prompts);
        }

        // GET: Prompts/Run/5
        public async Task<ActionResult> Run(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Prompts prompts = await db.Prompts.FindAsync(id);
            if (prompts == null)
            {
                return HttpNotFound();
            }
            return View(prompts);
        }

        // POST: Prompts/Run/5
        [HttpPost, ActionName("Run")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RunConfirmed(int id)
        {
            Prompts prompts = await db.Prompts.FindAsync(id);
            var openai = new OpenAIAPI(apiKey);
            var encoding = Tiktoken.Encoding.ForModel("gpt-4");
            CompletionRequest completion = new CompletionRequest();
            completion.Prompt = prompts.Prompt;
            completion.Model = OpenAI_API.Models.Model.DavinciText;
            completion.MaxTokens = 1024;
            var result = openai.Completions.CreateCompletionAsync(completion);
            if (result != null)
            {
                foreach (var item in result.Result.Completions)
                {
                    prompts.Result = item.Text;
                }
            }
            prompts.CompletionTokens = encoding.CountTokens(prompts.Result);
            prompts.TotalTokens += prompts.CompletionTokens + prompts.PromptTokens;
            db.Entry(prompts).State = EntityState.Modified;
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        // GET: Prompts/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Prompts prompts = await db.Prompts.FindAsync(id);
            if (prompts == null)
            {
                return HttpNotFound();
            }
            return View(prompts);
        }

        // POST: Prompts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Prompts prompts = await db.Prompts.FindAsync(id);
            db.Prompts.Remove(prompts);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
