﻿using CRM.WebApp.Infrastructure;
using CRM.WebApp.Models;
using EntityLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace CRM.WebApp.Controllers
{
    public class EmailSenderController : ApiController
    {
        EmailProvider provider = new EmailProvider();
        ApplicationManager manager = new ApplicationManager();
        [Route("api/EmailSender/{TemplateId}")]
        public async Task<IHttpActionResult> PostSendEmails([FromBody] List<Guid> GuIdList, [FromUri] int TemplateId)
        {
            List<ContactResponseModel> ContactsForSending = await manager.GetContactsByGuIdList(GuIdList);
            if (ContactsForSending == null)
            {
                return NotFound();
            }
            try
            {
                provider.SendEmailList(ContactsForSending, TemplateId);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);

            }
            return Ok();
        }
        [Route("api/EmailSender/{emailListId}/{TemplateId}")]
        public async Task<HttpResponseMessage> PostSendEmailList(int emailListId, int TemplateId)
        {
            if (!await manager.TemplateExistsAsync(TemplateId))
                return Request.CreateResponse(HttpStatusCode.NotFound);
            if (!await provider.SendMailToMailingList(emailListId,TemplateId))
                return Request.CreateResponse(HttpStatusCode.Conflict);
            return Request.CreateResponse(HttpStatusCode.OK);
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                manager.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
