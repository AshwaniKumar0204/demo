using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SchoolERP.Models
{
    public class DocumentModel
    {
        private int _DocumentID;
       public string UploadFileFormat { get; set; }

        private int _StudentId;

        private string _AddharCard;

        private string _MatrixCertifiacate;

        private string _IntermediateCertificate;

        private byte _DocumentStatus;

        private string _TenPassingCertificate;

        private string _TwelvePassingCertificate;

        private string _CLC;

        private string _MigrationCertificate;


        public int DocumentID { get => _DocumentID; set => _DocumentID = value; }
        public int StudentId { get => _StudentId; set => _StudentId = value; }
        public string AddharCard { get => _AddharCard; set => _AddharCard = value; }
        public string MatrixCertifiacate { get => _MatrixCertifiacate; set => _MatrixCertifiacate = value; }
        public string IntermediateCertificate { get => _IntermediateCertificate; set => _IntermediateCertificate = value; }
        public byte DocumentStatus { get => _DocumentStatus; set => _DocumentStatus = value; }
        public string TenPassingCertificate { get => _TenPassingCertificate; set => _TenPassingCertificate = value; }
        public string TwelvePassingCertificate { get => _TwelvePassingCertificate; set => _TwelvePassingCertificate = value; }
        public string CLC { get => _CLC; set => _CLC = value; }
        public string MigrationCertificate { get => _MigrationCertificate; set => _MigrationCertificate = value; }
    }
}