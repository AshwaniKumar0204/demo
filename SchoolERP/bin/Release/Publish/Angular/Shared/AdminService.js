(function () {
    "use strict";

    angular.module("app")
        .service("adminService", adminService);

    adminService.$inject = ["$http", "loadDataService"];

    function adminService($http, loadDataService) {
        //=====================Board Marks===============================
        this.updateRefundStatus = function (obj) {
            var response = $http({
                method: "post",
                url: "/api/Refund/updateRefundStatus",
                data: JSON.stringify(obj),
                dataType: "json",
            });
            return response;
        };
        this.getRefundList = function (obj) {
            var response = $http({
                method: "post",
                url: "/api/Refund/RefundList",
                data: JSON.stringify(obj),
                dataType: "json",
            });
            return response;
        };
        this.saveRefund = function (obj) {
            var response = $http({
                method: "post",
                url: "/api/Refund/SaveRefund",
                data: JSON.stringify(obj),
                dataType: "json",
            });
            return response;
        };

        //=====================Board Marks===============================
        this.updateBoardMarksSeen = function (obj) {
            var response = $http({
                method: "post",
                url: "/api/BoardMarks/UpdateBoardMarksSeen",
                data: JSON.stringify(obj),
                dataType: "json",
            });
            return response;
        };
        this.getBoardMarksList = function (obj) {
            var response = $http({
                method: "post",
                url: "/api/BoardMarks/BoardMarksList",
                data: JSON.stringify(obj),
                dataType: "json",
            });
            return response;
        };

        //============================================Staff Payment========================================================
        this.deleteStaffPayment = function (StaffPaymentId) {
            return $http.get("/api/StaffPayment/deleteStaffPayment", { params: { StaffPaymentId: StaffPaymentId } });
        }

        this.saveStaffPayment = function (StaffPaymentModel) {
            var response = $http({
                method: "post",
                url: "/api/StaffPayment/saveStaffPayment",
                data: JSON.stringify(StaffPaymentModel),
                dataType: "json",
            });
            return response;
        };

        this.getStaffPaymentDetail = function (obj) {
            var response = $http({
                method: "post",
                url: "/api/StaffPayment/StaffPaymentDetail",
                data: JSON.stringify(obj),
                dataType: "json",
            });
            return response;
        };

        this.getStaffPaymentList = function (obj) {
            var response = $http({
                method: "post",
                url: "/api/StaffPayment/StaffPaymentList",
                data: JSON.stringify(obj),
                dataType: "json",
            });
            return response;
        };

        //=========================================Staff Available Leave===========================================
        this.getStaffAvailableLeaves = function (StaffId) {
            return $http.get("/api/StaffAvailableLeave/StaffAvailableLeaves", { params: { StaffId: StaffId } });
        }
        this.getStaffAvailableLeaveList = function (leaveType, Year) {
            return $http.get("/api/StaffAvailableLeave/StaffAvailableLeaveList", { params: { leaveType: leaveType, Year: Year } });
        }

        //-------------------------Manage Vehicle Service---------------------------------------
        this.getExamResultPublishList = function (obj) {
            var response = $http({
                method: "post",
                url: "/api/ExamResultPublish/ExamResultPublishList",
                data: JSON.stringify(obj),
                dataType: "json",
            });
            return response;
        };

        this.deleteExamResultPublish = function (ExamResultPublishId) {
            return $http.get("/api/ExamResultPublish/deleteExamResultPublish", { params: { ExamResultPublishId: ExamResultPublishId } });
        }

        this.saveExamResultPublish = function (obj) {
            var response = $http({
                method: "post",
                url: "/api/ExamResultPublish/saveExamResultPublish",
                data: JSON.stringify(obj),
                dataType: "json",
            });
            return response;
        };


        //Attendence
        this.getAttendenceList = function (obj) {
            var response = $http({
                method: "post",
                url: "/api/Attendence/AttendenceList",
                data: JSON.stringify(obj),
                dataType: "json",
            });
            return response;
        };
        this.getAttendenceCodeList = function (obj) {
            var response = $http({
                method: "post",
                url: "/api/Attendence/AttendenceCodeList",
                data: JSON.stringify(obj),
                dataType: "json",
            });
            return response;
        };
        this.updateAttendenceCode = function (obj) {
            var response = $http({
                method: "post",
                url: "/api/Attendence/UpdateAttendenceCode",
                data: JSON.stringify(obj),
                dataType: "json",
            });
            return response;
        };

        //Manage Staff Group Shift
        this.getStaffGroupShiftList = function (StaffGroupId) {
            return $http.get("/api/StaffGroupShift/StaffGroupShiftList", { params: { StaffGroupId: StaffGroupId } });
        }

        this.AllStaffGroupShiftList = function (Month, Year) {
            return $http.get("/api/StaffGroupShift/AllStaffGroupShiftList", { params: { Month: Month, Year: Year } });
        }

        this.getStaffGroupShiftYearList = function () {
            return $http.get("/api/StaffGroupShift/StaffGroupShiftYearList");
        }

        this.saveStaffGroupShift = function (StaffGroupShifts) {
            var response = $http({
                method: "post",
                url: "/api/StaffGroupShift/saveStaffGroupShift",
                data: JSON.stringify(StaffGroupShifts),
                dataType: "json",
            });
            return response;
        };

        this.UpdateStaffGroupShift = function (StaffGroupShifts) {
            var response = $http({
                method: "post",
                url: "/api/StaffGroupShift/UpdateStaffGroupShift",
                data: JSON.stringify(StaffGroupShifts),
                dataType: "json",
            });
            return response;
        };

        this.deleteStaffGroupShift = function (StaffGroupShiftId) {
            return $http.get("/api/StaffGroupShift/deleteStaffGroupShift", { params: { StaffGroupShiftId: StaffGroupShiftId } });
        }

        //Manage Shift
        this.getShiftList = function () {
            return $http.get("/api/Shift/ShiftList");
        }

        this.deleteShift = function (ShiftId) {
            return $http.get("/api/Shift/deleteShift", { params: { ShiftId: ShiftId } });
        }

        this.saveShift = function (ShiftModel) {
            var response = $http({
                method: "post",
                url: "/api/Shift/saveShift",
                data: JSON.stringify(ShiftModel),
                dataType: "json",
            });
            return response;
        };


        //-------------------------Payment Link---------------------------------------
        this.getAdmissionFeeListForLink = function (AcademicSessionId, ClassNo) {
            return $http.get("/api/PaymentLink/AdmissionFeeListForLink", { params: { AcademicSessionId: AcademicSessionId, ClassNo: ClassNo } });
        }

        this.savePaymentLink = function (obj) {
            var response = $http({
                method: "post",
                url: "/api/PaymentLink/SavePaymentLink",
                data: JSON.stringify(obj),
                dataType: "json",
            });
            return response;
        };

        this.savePaymentLinkTwo = function (obj) {
            var response = $http({
                method: "post",
                url: "/api/PaymentLink/SavePaymentLinkTwo",
                data: JSON.stringify(obj),
                dataType: "json",
            });
            return response;
        };

        this.getRegistrationOnlineListForLinkTwo = function (obj) {
            var response = $http({
                method: "post",
                url: "/api/PaymentLink/RegistrationOnlineListForLinkTwo",
                data: JSON.stringify(obj),
                dataType: "json",
            });
            return response;
        };

        this.getPaidPaymentLinkList = function (obj) {
            var response = $http({
                method: "post",
                url: "/api/PaymentLink/PaidPaymentLinkList",
                data: JSON.stringify(obj),
                dataType: "json",
            });
            return response;
        };

        this.getPaymentLinkList = function (obj) {
            var response = $http({
                method: "post",
                url: "/api/PaymentLink/PaymentLinkList",
                data: JSON.stringify(obj),
                dataType: "json",
            });
            return response;
        };

        this.updatePaymentLink = function (obj) {
            var response = $http({
                method: "post",
                url: "/api/PaymentLink/UpdatePaymentLink",
                data: JSON.stringify(obj),
                dataType: "json",
            });
            return response;
        };

        //-------------------------Manage MagzinePurchase---------------------------------------
        this.getMagazinePurchaseList = function (MagazineId) {
            return $http.get("/api/MagazinePurchase/MagazinePurchaseList", { params: { MagazineId: MagazineId } });
        }

        this.deleteMagazinePurchase = function (MagazinePurchaseId) {
            return $http.get("/api/MagazinePurchase/deleteMagazinePurchase", { params: { MagazinePurchaseId: MagazinePurchaseId } });
        }

        this.saveMagazinePurchase = function (MagazinePurchaseModel) {
            var response = $http({
                method: "post",
                url: "/api/MagazinePurchase/saveMagazinePurchase",
                data: JSON.stringify(MagazinePurchaseModel),
                dataType: "json",
            });
            return response;
        };

        //-------------------------Manage Staff Salary---------------------------------------
        this.getStaffSalaryList = function (StaffId) {
            if (!StaffId)
                StaffId = 0;
            return $http.get("/api/StaffSalary/StaffSalaryList", { params: { StaffId: StaffId } });
        }

        this.deleteStaffSalary = function (StaffSalaryId) {
            return $http.get("/api/StaffSalary/deleteStaffSalary", { params: { StaffSalaryId: StaffSalaryId } });
        }

        this.saveStaffSalary = function (StaffSalaryModel) {
            var response = $http({
                method: "post",
                url: "/api/StaffSalary/saveStaffSalary",
                data: JSON.stringify(StaffSalaryModel),
                dataType: "json",
            });
            return response;
        };

        //-------------------------Manage Staff Leave---------------------------------------
        this.getStaffLeaveList = function (StaffLeaveModel) {
            var response = $http({
                method: "post",
                url: "/api/StaffLeave/StaffLeaveList",
                data: JSON.stringify(StaffLeaveModel),
                dataType: "json",
            });
            return response;
        };

        this.deleteStaffLeave = function (StaffLeaveId) {
            return $http.get("/api/StaffLeave/deleteStaffLeave", { params: { StaffLeaveId: StaffLeaveId } });
        }

        this.saveStaffLeave = function (StaffLeaveModel) {
            var response = $http({
                method: "post",
                url: "/api/StaffLeave/saveStaffLeave",
                data: JSON.stringify(StaffLeaveModel),
                dataType: "json",
            });
            return response;
        };

        //-------------------------Manage Staff Advance Payment---------------------------------------

        this.getStaffAdvancePaymentList = function (StaffAdvancePaymentModel) {
            var response = $http({
                method: "post",
                url: "/api/StaffAdvancePayment/StaffAdvancePaymentList",
                data: JSON.stringify(StaffAdvancePaymentModel),
                dataType: "json",
            });
            return response;
        };


        this.deleteStaffAdvancePayment = function (StaffAdvancePaymentId) {
            return $http.get("/api/StaffAdvancePayment/deleteStaffAdvancePayment", { params: { StaffAdvancePaymentId: StaffAdvancePaymentId } });
        }

        this.saveStaffAdvancePayment = function (StaffAdvancePaymentModel) {
            var response = $http({
                method: "post",
                url: "/api/StaffAdvancePayment/saveStaffAdvancePayment",
                data: JSON.stringify(StaffAdvancePaymentModel),
                dataType: "json",
            });
            return response;
        };

        //-------------------------Manage MagzinePurchase---------------------------------------
        this.getMagazinePurchaseList = function (MagazineId) {
            return $http.get("/api/MagazinePurchase/MagazinePurchaseList", { params: { MagazineId: MagazineId } });
        }

        this.deleteMagazinePurchase = function (MagazinePurchaseId) {
            return $http.get("/api/MagazinePurchase/deleteMagazinePurchase", { params: { MagazinePurchaseId: MagazinePurchaseId } });
        }

        this.saveMagazinePurchase = function (MagazinePurchaseModel) {
            var response = $http({
                method: "post",
                url: "/api/MagazinePurchase/saveMagazinePurchase",
                data: JSON.stringify(MagazinePurchaseModel),
                dataType: "json",
            });
            return response;
        };

        //-------------------------Manage Vehicle Service---------------------------------------
        this.getVehicleServiceList = function (DateFilerModel) {
            var response = $http({
                method: "post",
                url: "/api/VehicleService/VehicleServiceList",
                data: JSON.stringify(DateFilerModel),
                dataType: "json",
            });
            return response;
        };

        this.deleteVehicleService = function (VehicleServiceId) {
            return $http.get("/api/VehicleService/deleteVehicleService", { params: { VehicleServiceId: VehicleServiceId } });
        }

        this.getVehicleServiceDetail = function (VehicleServiceId) {
            return $http.get("/api/VehicleService/VehicleServiceDetail", { params: { VehicleServiceId: VehicleServiceId } });
        }

        this.saveVehicleService = function (VehicleServiceModel) {
            var response = $http({
                method: "post",
                url: "/api/VehicleService/saveVehicleService",
                data: JSON.stringify(VehicleServiceModel),
                dataType: "json",
            });
            return response;
        };

        //-------------------------Manage Vehicle Service Detail---------------------------------------
        this.deleteVehicleServiceDetail = function (VehicleServiceDetailId) {
            return $http.get("/api/VehicleServiceDetail/deleteVehicleServiceDetail", { params: { VehicleServiceDetailId: VehicleServiceDetailId } });
        }

        this.getVehicleServiceDetailList = function (VehicleServiceId) {
            return $http.get("/api/VehicleServiceDetail/VehicleServiceDetailList", { params: { VehicleServiceId: VehicleServiceId } });
        }

        this.getJobTypeList = function () {
            return $http.get("/api/VehicleServiceDetail/JobTypeList");
        }


        //----------------------------News Magazine----------------------
        this.saveMagazine = function (MagazineList) {
            var response = $http({
                method: "post",
                url: "/api/Magazine/SaveMagazine",
                data: JSON.stringify(MagazineList),
                dataType: "json",
            });
            return response;
        }

        this.getMagazineList = function (Status) {
            return $http.get("/api/Magazine/MagazineList", { params: { Status: Status } });
        }

        this.deleteMagazine = function (MagazineId) {
            return $http.get("/api/Magazine/deleteMagazine", { params: { MagazineId: MagazineId } });
        }


        //----------------------------Leave----------------------
        this.saveLeave = function (LeaveList) {
            var response = $http({
                method: "post",
                url: "/api/Leave/SaveLeave",
                data: JSON.stringify(LeaveList),
                dataType: "json",
            });
            return response;
        }

        this.getLeaveList = function (Status, staffType, Year) {
            if (Status == null || Status == undefined)
                Status = 0;
            if (staffType == null || staffType == undefined)
                staffType = 0;
            if (Year == null || Year == undefined)
                Year = 0;
            return $http.get("/api/Leave/LeaveList", { params: { Status: Status, staffType: staffType, Year: Year } });
        }

        this.deleteLeave = function (LeaveId) {
            return $http.get("/api/Leave/deleteLeave", { params: { LeaveId: LeaveId } });
        }

        this.getLeaveYearList = function () {
            return $http.get("/api/Leave/LeaveYearList");
        }
        //----------------------------News Parent----------------------
        this.saveParent = function (ParentList) {
            var response = $http({
                method: "post",
                url: "/api/Parent/SaveParent",
                data: JSON.stringify(ParentList),
                dataType: "json",
            });
            return response;
        }

        this.getParentList = function (Status) {
            return $http.get("/api/Parent/ParentList", { params: { Status: Status } });
        }

        this.deleteParent = function (ParentId) {
            return $http.get("/api/Parent/deleteParent", { params: { ParentId: ParentId } });
        }
        //----------------------------News Paper Purchase----------------------
        this.saveNewsPaperPurchase = function (NewsPaperPurchaseList) {
            var response = $http({
                method: "post",
                url: "/api/NewsPaperPurchase/SaveNewsPaperPurchase",
                data: JSON.stringify(NewsPaperPurchaseList),
                dataType: "json",
            });
            return response;
        }

        this.getNewPaperListForPurchase = function (purchaseDate) {
            purchaseDate = loadDataService.getDateMMDDYYYY(purchaseDate);
            return $http.get("/api/NewsPaperPurchase/NewPaperListForPurchase", { params: { purchaseDate: purchaseDate } });
        }

        this.getNewPaperPurchaseList = function (MonthNo, Year) {
            return $http.get("/api/NewsPaperPurchase/NewPaperPurchaseList", { params: { MonthNo: MonthNo, Year: Year } });
        }

        this.getNewsPaperYearList = function () {
            return $http.get("/api/NewsPaperPurchase/NewsPaperYearList");
        }

        //-----------------------------------Book Issue----------------------------------
        this.saveBookIssue = function (BookIssueModel) {
            var response = $http({
                method: "post",
                url: "/api/BookIssue/BookIssue",
                data: JSON.stringify(BookIssueModel),
                dataType: "json",
            });
            return response;
        }

        this.bookReturn = function (BookIssueModel) {
            var response = $http({
                method: "post",
                url: "/api/BookIssue/BookReturn",
                data: JSON.stringify(BookIssueModel),
                dataType: "json",
            });
            return response;
        }

        this.bookReissue = function (BookIssueModel) {
            var response = $http({
                method: "post",
                url: "/api/BookIssue/BookReissue",
                data: JSON.stringify(BookIssueModel),
                dataType: "json",
            });
            return response;
        }

        this.getBookIssueList = function (StudentId, StaffId) {
            return $http.get("/api/BookIssue/BookIssueList", { params: { StudentId: StudentId, StaffId: StaffId } });
        }

        this.deleteBookIssue = function (BookIssueId) {
            return $http.get("/api/BookIssue/deleteBookIssue", { params: { BookIssueId: BookIssueId } });
        }

        this.searchBookIssueList = function () {
            return $http.get("/api/BookIssue/SearchBookIssueList");
        }

        this.getBookIssueHistory = function (FromDate, ToDate, BookAccessionStatus) {
            FromDate = loadDataService.getDateMMDDYYYY(FromDate);
            ToDate = loadDataService.getDateMMDDYYYY(ToDate);
            BookAccessionStatus = loadDataService.getInt(BookAccessionStatus);
            return $http.get("/api/BookIssue/BookIssueHistory", { params: { FromDate: FromDate, ToDate: ToDate, BookAccessionStatus: BookAccessionStatus } });
        }

        //-----------------------------------Book Issue----------------------------------
        this.saveStoreItemIssue = function (StoreItemIssueModel) {
            var response = $http({
                method: "post",
                url: "/api/StoreItemIssue/StoreItemIssue",
                data: JSON.stringify(StoreItemIssueModel),
                dataType: "json",
            });
            return response;
        }

        this.storeItemReturn = function (StoreItemIssueModel) {
            var response = $http({
                method: "post",
                url: "/api/StoreItemIssue/StoreItemReturn",
                data: JSON.stringify(StoreItemIssueModel),
                dataType: "json",
            });
            return response;
        }

        this.getStoreItemIssueList = function (StudentId, StaffId, MiscellaneousStaffId) {
            return $http.get("/api/StoreItemIssue/StoreItemIssueList", { params: { StudentId: StudentId, StaffId: StaffId, MiscellaneousStaffId: MiscellaneousStaffId } });
        }

        this.deleteStoreItemIssue = function (StoreItemIssueId) {
            return $http.get("/api/StoreItemIssue/deleteStoreItemIssue", { params: { StoreItemIssueId: StoreItemIssueId } });
        }

        this.getStoreItemIssueHistory = function (FromDate, ToDate, IssueStatus) {
            FromDate = loadDataService.getDateMMDDYYYY(FromDate);
            ToDate = loadDataService.getDateMMDDYYYY(ToDate);
            IssueStatus = loadDataService.getInt(IssueStatus);
            return $http.get("/api/StoreItemIssue/StoreItemIssueHistory", { params: { FromDate: FromDate, ToDate: ToDate, IssueStatus: IssueStatus } });
        }

        //-----------------------------------Book Accession--------------------------------
        this.getBookAccessionList = function (ClassNo, Status, FromDate, ToDate) {
            var filterModel = {
                ClassNo: ClassNo,
                Status: Status,
                FromDate: loadDataService.getDateTime(FromDate),
                ToDate: loadDataService.getDateTime(ToDate)
            }
            var response = $http({
                method: "post",
                url: "/api/BookAccession/BookAccessionList",
                data: JSON.stringify(filterModel),
                dataType: "json",
            });
            return response;
        }
        this.saveBookAccession = function (BookAccessionModel) {
            var response = $http({
                method: "post",
                url: "/api/BookAccession/SaveBookAccession",
                data: JSON.stringify(BookAccessionModel),
                dataType: "json",
            });
            return response;
        }

        this.setSessionForPrintBarcode = function (obj) {
            var response = $http({
                method: "post",
                url: "/api/Session/SetSessionForPrintBarcode",
                data: JSON.stringify(obj),
                dataType: "json",
            });
            return response;
        }

        this.getSearchBookAccessionList = function (BookStatus, BookAccessionStatus) {
            return $http.get("/api/BookAccession/SearchBookAccessionList", { params: { BookStatus: BookStatus, BookAccessionStatus: BookAccessionStatus } });
        }

        this.getBookAccessionListWithRack = function (AccessionNo) {
            AccessionNo = loadDataService.getInt(AccessionNo);
            return $http.get("/api/BookAccession/BookAccessionListWithRack", { params: { AccessionNo: AccessionNo } });
        }

        this.getBookAccessionDetail = function (AccessionNo) {
            return $http.get("/api/BookAccession/BookAccessionDetail", { params: { AccessionNo: AccessionNo } });
        }

        //-----------------------------------Book--------------------------------
        this.getSearchBookList = function (status) {
            if (status == null)
                status = 0;
            return $http.get("/api/Book/SearchBookList", { params: { status: status } });
        }
        this.getSearchBookListClasswise = function (status, ClassNo) {
            if (status == null)
                status = 0;
            return $http.get("/api/Book/SearchBookListClasswise", { params: { status: status, ClassNo: ClassNo } });
        }
        this.getBookList = function (Status, ClassNo, BookTypeId) {
            if (Status == null || Status == undefined)
                Status = 0;
            if (ClassNo == null || ClassNo == undefined)
                ClassNo = 0;
            if (BookTypeId == null || BookTypeId == undefined)
                BookTypeId = 0;
            return $http.get("/api/Book/BookList", { params: { Status: Status, ClassNo: ClassNo, BookTypeId: BookTypeId } });
        }

        this.deleteBook = function (BookId) {
            return $http.get("/api/Book/deleteBook", { params: { BookId: BookId } });
        }

        this.saveBook = function (BookModel) {
            var response = $http({
                method: "post",
                url: "/api/Book/SaveBook",
                data: JSON.stringify(BookModel),
                dataType: "json",
            });
            return response;
        };

        this.getAuthorList = function () {
            return $http.get("/api/Book/AuthorList");
        }

        this.getPublisherList = function () {
            return $http.get("/api/Book/PublisherList");
        }

        this.getSubjectTypeList = function () {
            return $http.get("/api/Book/SubjectTypeList");
        }

        //-----------------------------------Purchase--------------------------------
        this.savePurchase = function (PurchaseModel) {
            var response = $http({
                method: "post",
                url: "/api/Purchase/SavePurchase",
                data: JSON.stringify(PurchaseModel),
                dataType: "json",
            });
            return response;
        };

        this.getPurchaseList = function (fromDate, toDate, SupplierId, PurchaseType) {
            fromDate = loadDataService.getDateMMDDYYYY(fromDate);
            toDate = loadDataService.getDateMMDDYYYY(toDate);
            if (SupplierId == null)
                SupplierId = 0;
            PurchaseType = loadDataService.getInt(PurchaseType);
            return $http.get("/api/Purchase/PurchaseList", { params: { fromDate: fromDate, toDate: toDate, SupplierId: SupplierId, PurchaseType: PurchaseType } });
        }

        //--------------------------------Purchase Book---------------------------------------
        this.getPurchaseBookList = function (PurchaseId) {
            return $http.get("/api/PurchaseBook/PurchaseBookList", { params: { PurchaseId: PurchaseId } });
        }

        //--------------------------------Purchase Store Item---------------------------------------
        this.getPurchaseStoreItemList = function (PurchaseId) {
            return $http.get("/api/PurchaseStoreItem/PurchaseStoreItemList", { params: { PurchaseId: PurchaseId } });
        }

        //-------------------------Registration Online ---------------------------------------
        this.saveRegistrationOnline = function (obj) {
            var response = $http({
                method: "post",
                url: "/api/RegistrationOnline/saveRegistrationOnline",
                data: JSON.stringify(obj),
                dataType: "json",
            });
            return response;
        };

        this.saveRegistrationOffline = function (obj) {
            var response = $http({
                method: "post",
                url: "/api/RegistrationOnline/saveRegistrationOffline",
                data: JSON.stringify(obj),
                dataType: "json",
            });
            return response;
        };

        this.getRegistrationOnlineList = function (obj) {
            var response = $http({
                method: "post",
                url: "/api/RegistrationOnline/RegistrationOnlineList",
                data: JSON.stringify(obj),
                dataType: "json",
            });
            return response;
        }

        this.getRegistrationOnlineListForAdmissionDetail = function (obj) {
            var response = $http({
                method: "post",
                url: "/api/RegistrationOnline/RegistrationOnlineListForAdmissionDetail",
                data: JSON.stringify(obj),
                dataType: "json",
            });
            return response;
        }

        this.getRegistrationOnlineListForGenerateLink = function (obj) {
            var response = $http({
                method: "post",
                url: "/api/RegistrationOnline/RegistrationOnlineListForGenerateLink",
                data: JSON.stringify(obj),
                dataType: "json",
            });
            return response;
        }

        this.getRegistrationOnlineListForAdmitCard = function (obj) {
            var response = $http({
                method: "post",
                url: "/api/RegistrationOnline/RegistrationOnlineListForAdmitCard",
                data: JSON.stringify(obj),
                dataType: "json",
            });
            return response;
        }

        this.getRegistrationOnlineListForGenerateAdmitCard = function (obj) {
            var response = $http({
                method: "post",
                url: "/api/RegistrationOnline/RegistrationOnlineListForGenerateAdmitCard",
                data: JSON.stringify(obj),
                dataType: "json",
            });
            return response;
        }

        this.generateAdmitCardOR = function (obj) {
            var response = $http({
                method: "post",
                url: "/api/RegistrationOnline/GenerateAdmitCardOR",
                data: JSON.stringify(obj),
                dataType: "json",
            });
            return response;
        }

        this.sendAdmitCardSMSRO = function (obj) {
            var response = $http({
                method: "post",
                url: "/api/RegistrationOnline/SendAdmitCardSMSRO",
                data: JSON.stringify(obj),
                dataType: "json",
            });
            return response;
        }

        this.sendAdmitCardSMSRO = function (obj) {
            var response = $http({
                method: "post",
                url: "/api/RegistrationOnline/SendAdmitCardSMSRO",
                data: JSON.stringify(obj),
                dataType: "json",
            });
            return response;
        }

        this.getRegistrationOnlineDetail = function (RegistrationOnlineId) {
            return $http.get("/api/RegistrationOnline/RegistrationOnlineDetail", { params: { RegistrationOnlineId: RegistrationOnlineId } });
        }

        this.getSearchRegistrationOnlineList = function () {
            return $http.get("/api/RegistrationOnline/SearchRegistrationOnlineList");
        }
        this.getRegistrationOnlineListForUpdateStreamType = function (obj) {
            var response = $http({
                method: "post",
                url: "/api/RegistrationOnline/RegistrationOnlineListForUpdateStreamType",
                data: JSON.stringify(obj),
                dataType: "json",
            });
            return response;
        };


        this.getRegistrationOnlineListForResult = function (obj) {
            var response = $http({
                method: "post",
                url: "/api/RegistrationOnline/RegistrationOnlineListForResult",
                data: JSON.stringify(obj),
                dataType: "json",
            });
            return response;
        };


        this.getRegistrationOnlineResultList = function (obj) {
            var response = $http({
                method: "post",
                url: "/api/RegistrationOnline/RegistrationOnlineResultList",
                data: JSON.stringify(obj),
                dataType: "json",
            });
            return response;
        };


        this.saveRegistrationOnlineResult = function (obj) {
            var response = $http({
                method: "post",
                url: "/api/RegistrationOnline/SaveRegistrationOnlineResult",
                data: JSON.stringify(obj),
                dataType: "json",
            });
            return response;
        };



        //-------------------------Manage Registration Form---------------------------------------
        this.getRegistrationFormList = function (Status, AcademicSessionId, ClassNo) {
            if (ClassNo == null)
                ClassNo = 0;
            return $http.get("/api/RegistrationForm/RegistrationFormList", { params: { AcademicSessionId: AcademicSessionId, Status: Status, ClassNo: ClassNo } });
        }
        this.getSearchRegistrationFormList = function (Status, AcademicSessionId, ClassNo) {
            if (Status == null)
                Status = 0;
            if (AcademicSessionId == null)
                AcademicSessionId = 0;
            if (ClassNo == null)
                ClassNo = 0;
            return $http.get("/api/RegistrationForm/SearchRegistrationFormList", { params: { AcademicSessionId: AcademicSessionId, Status: Status, ClassNo: ClassNo } });
        }

        this.deleteRegistrationForm = function (RegistrationFormId) {
            return $http.get("/api/RegistrationForm/deleteRegistrationForm", { params: { RegistrationFormId: RegistrationFormId } });
        }

        this.saveRegistrationForm = function (RegistrationFormModel) {
            var response = $http({
                method: "post",
                url: "/api/RegistrationForm/saveRegistrationForm",
                data: JSON.stringify(RegistrationFormModel),
                dataType: "json",
            });
            return response;
        };

        //----------------------------------------------------Transfer Certificate----------------------------------------------------
        this.getUnissuedTransferCertificateAdmissionList = function () {
            return $http.get("/api/TransferCertificate/UnissuedTransferCertificateAdmissionList");
        }

        this.getTransferCertificateList = function (AcademicSessionId, ClassId, SectionId, TransferCertificateId) {
            if (TransferCertificateId == null || TransferCertificateId == undefined)
                TransferCertificateId = 0;
            return $http.get("/api/TransferCertificate/TransferCertificateList", {
                params:
                {
                    AcademicSessionId: AcademicSessionId,
                    ClassId: ClassId,
                    SectionId: SectionId,
                    TransferCertificateId: TransferCertificateId
                }
            });
        }

        this.getStudentConcessionDetail = function (StudentId) {
            return $http.get("/api/TransferCertificate/StudentConcessionDetail", {
                params: { StudentId: StudentId }
            });
        }

        this.saveTransferCertificate = function (TransferCertificateModel) {
            var response = $http({
                method: "post",
                url: "/api/TransferCertificate/saveTransferCertificate",
                data: JSON.stringify(TransferCertificateModel),
                dataType: "json",
            });
            return response;
        };


        //----------------------------------------------------Character Certificate----------------------------------------------------
        this.getUnissuedCharacterCertificateAdmissionList = function () {
            return $http.get("/api/CharacterCertificate/UnissuedCharacterCertificateAdmissionList");
        }

        this.getCharacterCertificateList = function (AcademicSessionId, ClassId, SectionId, CharacterCertificateId) {
            if (CharacterCertificateId == null || CharacterCertificateId == undefined)
                CharacterCertificateId = 0;
            return $http.get("/api/CharacterCertificate/CharacterCertificateList", {
                params:
                {
                    AcademicSessionId: AcademicSessionId,
                    ClassId: ClassId,
                    SectionId: SectionId,
                    CharacterCertificateId: CharacterCertificateId
                }
            });
        }

        this.saveCharacterCertificate = function (CharacterCertificateModel) {
            var response = $http({
                method: "post",
                url: "/api/CharacterCertificate/saveCharacterCertificate",
                data: JSON.stringify(CharacterCertificateModel),
                dataType: "json",
            });
            return response;
        };


        //----------------------------------------------------Bonafide Certificate----------------------------------------------------
        this.getUnissuedBonafideCertificateAdmissionList = function () {
            return $http.get("/api/BonafideCertificate/UnissuedBonafideCertificateAdmissionList");
        }

        this.getBonafideCertificateList = function (AcademicSessionId, ClassId, SectionId, BonafideCertificateId) {
            if (BonafideCertificateId == null || BonafideCertificateId == undefined)
                BonafideCertificateId = 0;
            return $http.get("/api/BonafideCertificate/BonafideCertificateList", {
                params:
                {
                    AcademicSessionId: AcademicSessionId,
                    ClassId: ClassId,
                    SectionId: SectionId,
                    BonafideCertificateId: BonafideCertificateId
                }
            });
        }

        this.saveBonafideCertificate = function (BonafideCertificateModel) {
            var response = $http({
                method: "post",
                url: "/api/BonafideCertificate/saveBonafideCertificate",
                data: JSON.stringify(BonafideCertificateModel),
                dataType: "json",
            });
            return response;
        };

        //----------------------------------------------------Menu----------------------------------------------------
        this.getUserMenu = function () {
            return $http.get("/api/menu/UserMenu");
        }
        //----------------------------------------------------School----------------------------------------------------
        this.getSchoolList = function (Status) {
            return $http.get("/api/school/SchoolList", { params: { Status: Status } });
        }


        //-------------------------Manage Teacher---------------------------------------
        this.getSearchTeacherList = function (Status) {
            return $http.get("/api/Teacher/SearchTeacherList", { params: { Status: Status } });
        }
        this.getTeacherList = function (Status) {
            return $http.get("/api/Teacher/TeacherList", { params: { Status: Status } });
        }

        this.deleteTeacher = function (TeacherId) {
            return $http.get("/api/Teacher/deleteTeacher", { params: { TeacherId: TeacherId } });
        }
        this.getPeriodList = function () {
            return $http.get("/api/Teacher/PeriodList");
        }
        this.deleteTeacherPeriod = function (TeacherPeriodId) {
            return $http.get("/api/Teacher/deleteTeacherPeriod", { params: { TeacherPeriodId: TeacherPeriodId } });
        }

        this.saveTeacher = function (TeacherModel) {
            var response = $http({
                method: "post",
                url: "/api/Teacher/saveTeacher",
                data: JSON.stringify(TeacherModel),
                dataType: "json",
            });
            return response;
        };


        //-------------------------Manage Subject---------------------------------------
        this.getSubjectList = function (Status) {
            return $http.get("/api/subject/SubjectList", { params: { Status: Status } });
        }

        this.deleteSubject = function (SubjectId) {
            return $http.get("/api/Subject/deleteSubject", { params: { SubjectId: SubjectId } });
        }

        this.saveSubject = function (SubjectModel) {
            var response = $http({
                method: "post",
                url: "/api/Subject/saveSubject",
                data: JSON.stringify(SubjectModel),
                dataType: "json",
            });
            return response;
        };


        //-------------------------Manage Staff---------------------------------------
        this.getStaffList = function (Status, DepartmentId) {
            if (Status == null || Status == undefined)
                Status = 0;
            if (DepartmentId == null || DepartmentId == undefined)
                DepartmentId = 0;
            return $http.get("/api/Staff/StaffList", { params: { Status: Status, DepartmentId: DepartmentId } });
        }

        this.deleteStaff = function (StaffId) {
            return $http.get("/api/Staff/deleteStaff", { params: { StaffId: StaffId } });
        }

        this.getSearchStaffList = function (status) {
            return $http.get("/api/Staff/SearchStaffList", { params: { status: status } });
        }

        this.saveStaff = function (StaffModel) {
            var response = $http({
                method: "post",
                url: "/api/Staff/saveStaff",
                data: JSON.stringify(StaffModel),
                dataType: "json",
            });
            return response;
        };


        //-------------------------Manage Miscellaneous Staff---------------------------------------
        this.getMiscellaneousStaffList = function () {
            return $http.get("/api/MiscellaneousStaff/MiscellaneousStaffList");
        }

        this.saveMiscellaneousStaff = function (MiscellaneousStaffModel) {
            var response = $http({
                method: "post",
                url: "/api/MiscellaneousStaff/saveMiscellaneousStaff",
                data: JSON.stringify(MiscellaneousStaffModel),
                dataType: "json",
            });
            return response;
        };

        //-------------------------Enumerator---------------------------------------
        this.getGenderList = function () {
            return $http.get("/api/Enumerator/GenderList");
        }
        this.getStaffTypeList = function () {
            return $http.get("/api/Enumerator/StaffTypeList");
        }
        this.getStatusList = function () {
            return $http.get("/api/Enumerator/StatusList");
        }
        this.getEntryTypeList = function () {
            return $http.get("/api/Enumerator/EntryTypeList");
        }
        this.getCategoryList = function () {
            return $http.get("/api/Enumerator/CategoryList");
        }
        this.getPassingStatusList = function () {
            return $http.get("/api/Enumerator/PassingStatusList");
        }
        this.getNationalityList = function () {
            return $http.get("/api/Enumerator/NationalityList");
        }
        this.getRefundStatusList = function () {
            return $http.get("/api/Enumerator/RefundStatusList");
        }
        this.getBoardMarksStatusList = function () {
            return $http.get("/api/Enumerator/BoardMarksStatusList");
        }
        this.getBookReadTypeList = function () {
            return $http.get("/api/Enumerator/BookReadTypeList");
        }
        this.getAbsentTypeList = function () {
            return $http.get("/api/Enumerator/AbsentTypeList");
        }
        this.getLeaveTypeList = function () {
            return $http.get("/api/Enumerator/LeaveTypeList");
        }
        this.getAdmissionFromList = function () {
            return $http.get("/api/Enumerator/AdmissionFromList");
        }
        this.getResultTypeList = function () {
            return $http.get("/api/Enumerator/ResultTypeList");
        }
        this.getDeviceTypeList = function () {
            return $http.get("/api/Enumerator/DeviceTypeList");
        }
        this.getVehicleServiceTypeList = function () {
            return $http.get("/api/Enumerator/VehicleServiceTypeList");
        }
        this.getSupplierTypeList = function () {
            return $http.get("/api/Enumerator/SupplierTypeList");
        }
        this.getBookIssueTypeList = function () {
            return $http.get("/api/Enumerator/BookIssueTypeList");
        }
        this.getMediumList = function () {
            return $http.get("/api/Enumerator/MediumList");
        }
        this.getBookPositionList = function () {
            return $http.get("/api/Enumerator/BookPositionList");
        }
        this.getEnumMonthList = function () {
            return $http.get("/api/Enumerator/MonthList");
        }


        //-------------------------Manage Department---------------------------------------
        this.getDepartmentList = function (Status) {
            return $http.get("/api/Department/DepartmentList", { params: { Status: Status } });
        }

        this.deleteDepartment = function (DepartmentId) {
            return $http.get("/api/Department/deleteDepartment", { params: { DepartmentId: DepartmentId } });
        }

        this.saveDepartment = function (DepartmentModel) {
            var response = $http({
                method: "post",
                url: "/api/Department/saveDepartment",
                data: JSON.stringify(DepartmentModel),
                dataType: "json",
            });
            return response;
        };


        //-------------------------Manage Book Type---------------------------------------
        this.getBookTypeList = function (Status) {
            return $http.get("/api/BookType/BookTypeList", { params: { Status: Status } });
        }

        this.deleteBookType = function (BookTypeId) {
            return $http.get("/api/BookType/deleteBookType", { params: { BookTypeId: BookTypeId } });
        }

        this.saveBookType = function (BookTypeModel) {
            var response = $http({
                method: "post",
                url: "/api/BookType/saveBookType",
                data: JSON.stringify(BookTypeModel),
                dataType: "json",
            });
            return response;
        };


        //-------------------------Manage Vehicle Type---------------------------------------
        this.getVehicleTypeList = function (Status) {
            return $http.get("/api/VehicleType/VehicleTypeList", { params: { Status: Status } });
        }

        this.deleteVehicleType = function (VehicleTypeId) {
            return $http.get("/api/VehicleType/deleteVehicleType", { params: { VehicleTypeId: VehicleTypeId } });
        }

        this.saveVehicleType = function (VehicleTypeModel) {
            var response = $http({
                method: "post",
                url: "/api/VehicleType/saveVehicleType",
                data: JSON.stringify(VehicleTypeModel),
                dataType: "json",
            });
            return response;
        };


        //-------------------------Manage Vehicle Type---------------------------------------
        this.getVehicleRunningRecordList = function (dateFileterModel) {
            var response = $http({
                method: "post",
                url: "/api/VehicleRunningRecord/VehicleRunningRecordList",
                data: JSON.stringify(dateFileterModel),
                dataType: "json",
            });
            return response;
        }

        this.deleteVehicleRunningRecord = function (VehicleRunningRecordId) {
            return $http.get("/api/VehicleRunningRecord/deleteVehicleRunningRecord", { params: { VehicleRunningRecordId: VehicleRunningRecordId } });
        }

        this.saveVehicleRunningRecord = function (VehicleRunningRecordModel) {
            var response = $http({
                method: "post",
                url: "/api/VehicleRunningRecord/saveVehicleRunningRecord",
                data: JSON.stringify(VehicleRunningRecordModel),
                dataType: "json",
            });
            return response;
        };


        //-------------------------Manage Book Subject---------------------------------------
        this.getBookSubjectList = function (Status) {
            return $http.get("/api/BookSubject/BookSubjectList", { params: { Status: Status } });
        }

        this.deleteBookSubject = function (BookSubjectId) {
            return $http.get("/api/BookSubject/deleteBookSubject", { params: { BookSubjectId: BookSubjectId } });
        }

        this.saveBookSubject = function (BookSubjectModel) {
            var response = $http({
                method: "post",
                url: "/api/BookSubject/saveBookSubject",
                data: JSON.stringify(BookSubjectModel),
                dataType: "json",
            });
            return response;
        };


        //-------------------------Manage Item Type---------------------------------------
        this.getItemTypeList = function (Status) {
            return $http.get("/api/ItemType/ItemTypeList", { params: { Status: Status } });
        }

        this.deleteItemType = function (ItemTypeId) {
            return $http.get("/api/ItemType/deleteItemType", { params: { ItemTypeId: ItemTypeId } });
        }

        this.saveItemType = function (ItemTypeModel) {
            var response = $http({
                method: "post",
                url: "/api/ItemType/saveItemType",
                data: JSON.stringify(ItemTypeModel),
                dataType: "json",
            });
            return response;
        };


        //-------------------------Manage Store Item---------------------------------------
        this.getStoreItemList = function (Status, ItemTypeId) {
            if (ItemTypeId == null || ItemTypeId == undefined)
                ItemTypeId = 0;
            return $http.get("/api/StoreItem/StoreItemList", { params: { Status: Status, ItemTypeId: ItemTypeId } });
        }

        this.deleteStoreItem = function (StoreItemId) {
            return $http.get("/api/StoreItem/deleteStoreItem", { params: { StoreItemId: StoreItemId } });
        }

        this.saveStoreItem = function (StoreItemModel) {
            var response = $http({
                method: "post",
                url: "/api/StoreItem/saveStoreItem",
                data: JSON.stringify(StoreItemModel),
                dataType: "json",
            });
            return response;
        };


        //-------------------------Manage GST---------------------------------------
        this.getGSTList = function (Status) {
            return $http.get("/api/GST/GSTList", { params: { Status: Status } });
        }

        this.deleteGST = function (GSTId) {
            return $http.get("/api/GST/deleteGST", { params: { GSTId: GSTId } });
        }

        this.saveGST = function (GSTModel) {
            var response = $http({
                method: "post",
                url: "/api/GST/saveGST",
                data: JSON.stringify(GSTModel),
                dataType: "json",
            });
            return response;
        };


        //-------------------------Manage Unit---------------------------------------
        this.getUnitList = function (Status) {
            return $http.get("/api/Unit/UnitList", { params: { Status: Status } });
        }

        this.deleteUnit = function (UnitId) {
            return $http.get("/api/Unit/deleteUnit", { params: { UnitId: UnitId } });
        }

        this.saveUnit = function (UnitModel) {
            var response = $http({
                method: "post",
                url: "/api/Unit/saveUnit",
                data: JSON.stringify(UnitModel),
                dataType: "json",
            });
            return response;
        };


        //-------------------------Manage Month---------------------------------------
        this.getMonthList = function (ClassId) {
            return $http.get("/api/Month/MonthList", { params: { ClassId: ClassId } });
        }

        this.deleteMonth = function (MonthId) {
            return $http.get("/api/Month/deleteMonth", { params: { MonthId: MonthId } });
        }

        this.saveMonth = function (MonthModel) {
            var response = $http({
                method: "post",
                url: "/api/Month/saveMonth",
                data: JSON.stringify(MonthModel),
                dataType: "json",
            });
            return response;
        };


        //-------------------------Manage StaffGroup---------------------------------------
        this.getStaffGroup = function (StaffGroupId) {
            return $http.get("/api/StaffGroup/StaffGroup", { params: { StaffGroupId: StaffGroupId } });
        }

        this.getStaffGroupList = function (Status) {
            return $http.get("/api/StaffGroup/StaffGroupList", { params: { Status: Status } });
        }

        this.deleteStaffGroup = function (StaffGroupId) {
            return $http.get("/api/StaffGroup/deleteStaffGroup", { params: { StaffGroupId: StaffGroupId } });
        }

        this.saveStaffGroup = function (StaffGroupModel) {
            var response = $http({
                method: "post",
                url: "/api/StaffGroup/saveStaffGroup",
                data: JSON.stringify(StaffGroupModel),
                dataType: "json",
            });
            return response;
        };


        //-------------------------Manage Device---------------------------------------
        this.getDeviceList = function (Status) {
            return $http.get("/api/Device/DeviceList", { params: { Status: Status } });
        }

        this.deleteDevice = function (DeviceId) {
            return $http.get("/api/Device/deleteDevice", { params: { DeviceId: DeviceId } });
        }

        this.saveDevice = function (DeviceModel) {
            var response = $http({
                method: "post",
                url: "/api/Device/saveDevice",
                data: JSON.stringify(DeviceModel),
                dataType: "json",
            });
            return response;
        };


        //-------------------------Manage ExamCenter---------------------------------------
        this.getExamCenterList = function (Status) {
            return $http.get("/api/ExamCenter/ExamCenterList", { params: { Status: Status } });
        }

        this.deleteExamCenter = function (ExamCenterId) {
            return $http.get("/api/ExamCenter/deleteExamCenter", { params: { ExamCenterId: ExamCenterId } });
        }

        this.saveExamCenter = function (ExamCenterModel) {
            var response = $http({
                method: "post",
                url: "/api/ExamCenter/saveExamCenter",
                data: JSON.stringify(ExamCenterModel),
                dataType: "json",
            });
            return response;
        };


        //-------------------------Manage Designation---------------------------------------
        this.getDesignationList = function (Status) {
            return $http.get("/api/Designation/DesignationList", { params: { Status: Status } });
        }

        this.deleteDesignation = function (DesignationId) {
            return $http.get("/api/Designation/deleteDesignation", { params: { DesignationId: DesignationId } });
        }

        this.saveDesignation = function (DesignationModel) {
            var response = $http({
                method: "post",
                url: "/api/Designation/saveDesignation",
                data: JSON.stringify(DesignationModel),
                dataType: "json",
            });
            return response;
        };

        //-------------------------Manage BookSubjectRack---------------------------------------
        this.getBookSubjectRackList = function (BookSubjectId, RackId, ClassNo) {
            BookSubjectId = loadDataService.getInt(BookSubjectId);
            RackId = loadDataService.getInt(RackId);
            ClassNo = loadDataService.getInt(ClassNo);
            return $http.get("/api/BookSubjectRack/BookSubjectRackList", { params: { BookSubjectId: BookSubjectId, RackId: RackId, ClassNo: ClassNo } });
        }

        this.deleteBookSubjectRack = function (BookSubjectRackId) {
            return $http.get("/api/BookSubjectRack/deleteBookSubjectRack", { params: { BookSubjectRackId: BookSubjectRackId } });
        }

        this.saveBookSubjectRack = function (BookSubjectRackModel) {
            var response = $http({
                method: "post",
                url: "/api/BookSubjectRack/saveBookSubjectRack",
                data: JSON.stringify(BookSubjectRackModel),
                dataType: "json",
            });
            return response;
        };


        //-------------------------Manage Rack---------------------------------------
        this.getRackList = function (Status) {
            return $http.get("/api/Rack/RackList", { params: { Status: Status } });
        }

        this.deleteRack = function (RackId) {
            return $http.get("/api/Rack/deleteRack", { params: { RackId: RackId } });
        }

        this.saveRack = function (RackModel) {
            var response = $http({
                method: "post",
                url: "/api/Rack/saveRack",
                data: JSON.stringify(RackModel),
                dataType: "json",
            });
            return response;
        };



        //-------------------------Manage Class Subject---------------------------------------
        this.getClassSubjectList = function (SectionId, Status, AcademicSessionId) {
            return $http.get("/api/ClassSubject/ClassSubjectList", { params: { SectionId: SectionId, Status: Status, AcademicSessionId: AcademicSessionId } });
        }

        this.getClassSubjectTeacherList = function (ClassId, SectionId, AcademicSessionId) {
            return $http.get("/api/ClassSubject/ClassSubjectTeacherList", { params: { ClassId: ClassId, SectionId: SectionId, AcademicSessionId: AcademicSessionId } });
        }

        this.deleteClassSubject = function (ClassSubjectId) {
            return $http.get("/api/ClassSubject/deleteClassSubject", { params: { ClassSubjectId: ClassSubjectId } });
        }

        this.changePositionUp = function (ClassSubjectId) {
            return $http.get("/api/ClassSubject/changePositionUp", { params: { ClassSubjectId: ClassSubjectId } });
        }

        this.changePositionDown = function (ClassSubjectId) {
            return $http.get("/api/ClassSubject/changePositionDown", { params: { ClassSubjectId: ClassSubjectId } });
        }

        this.copyClassSubjects = function (SectionId, AcademicSessionId) {
            return $http.get("/api/ClassSubject/CopyClassSubjects", { params: { SectionId: SectionId, AcademicSessionId: AcademicSessionId } });
        }

        this.saveClassSubject = function (ClassSubjectModel) {
            var response = $http({
                method: "post",
                url: "/api/ClassSubject/saveClassSubject",
                data: JSON.stringify(ClassSubjectModel),
                dataType: "json",
            });
            return response;
        };

        //--------------------------------------Student Left---------------------------------
        this.getStudentPreviousDues = function (AdmissionId) {
            return $http.get("/api/student/StudentPreviousDues", { params: { AdmissionId: AdmissionId } });
        }
        this.getLeftStudentList = function (Status) {
            return $http.get("/api/leftstudent/leftStudentList", { params: { Status: Status } });
        }
        this.rejoinLeftStudent = function (LeftStudentId) {
            return $http.get("/api/leftstudent/rejoinLeftStudent", { params: { LeftStudentId: LeftStudentId } });
        }

        this.leftStudent = function (leftStudentModel) {
            var response = $http({
                method: "post",
                url: "/api/leftstudent/leftStudent",
                data: JSON.stringify(leftStudentModel),
                dataType: "json",
            });
            return response;
        }

        //------------------------------------Cashbook----------------------------------------
        this.getCashbookDetail = function (PaymenDate) {
            return $http.get("/api/cashbook/cashbookDetail", { params: { PaymenDate: PaymenDate } });
        }

        this.saveCashbook = function (cashbookList) {
            var response = $http({
                method: "post",
                url: "/api/cashbook/saveCashbook",
                data: JSON.stringify(cashbookList),
                dataType: "json",
            });
            return response;
        }

        this.getChequebookDetail = function (PaymenDate) {
            return $http.get("/api/chequebook/chequebookDetail", { params: { PaymenDate: PaymenDate } });
        }
        this.saveChequebook = function (chequebookList) {
            var response = $http({
                method: "post",
                url: "/api/chequebook/saveChequebook",
                data: JSON.stringify(chequebookList),
                dataType: "json",
            });
            return response;
        }




        //------------------------------------Fee Payment Cancle----------------------------------------
        this.getFeeCollectionDetailByReceiptNo = function (ReceiptNo) {
            return $http.get("/api/feePayment/FeeCollectionDetailByReceiptNo", { params: { ReceiptNo: ReceiptNo } });
        }

        this.feePaymentCancle = function (feePaymentModel) {
            var response = $http({
                method: "post",
                url: "/api/feePayment/FeePaymentCancle",
                data: JSON.stringify(feePaymentModel),
                dataType: "json",
            });
            return response;
        }


        //------------------------------------Update Fee Payment Status----------------------------------------
        this.getPendingFeePaymentTempList = function (FromDate, ToDate, AdmissionId) {
            return $http.get("/api/feePayment/PendingFeePaymentTempList", { params: { FromDate: FromDate, ToDate: ToDate, AdmissionId: AdmissionId } });
        }
        this.getSuccessTempPaymentList = function (FromDate, ToDate, AdmissionId) {
            return $http.get("/api/feePayment/SuccessTempPaymentList", { params: { FromDate: FromDate, ToDate: ToDate, AdmissionId: AdmissionId } });
        }

        this.checkOnlineFeePaymentStatus = function (feeCollectionTempList) {
            var response = $http({
                method: "post",
                url: "/api/feePayment/CheckOnlineFeePaymentStatus",
                data: JSON.stringify(feeCollectionTempList),
                dataType: "json",
            });
            return response;
        }

        this.autoUpdateFeePaymentStatus = function (feeCollectionModel) {
            var response = $http({
                method: "post",
                url: "/api/feePayment/AutoUpdateFeePaymentStatus",
                data: JSON.stringify(feeCollectionModel),
                dataType: "json",
            });
            return response;
        }

        this.updateStudentFeePayment = function (UpdateFeePaymentStatusModel) {
            var response = $http({
                method: "post",
                url: "/api/feePayment/UpdateStudentFeePayment",
                data: JSON.stringify(UpdateFeePaymentStatusModel),
                dataType: "json",
            });
            return response;
        }

        //----------------------------Student Concession-----------------------------------------------
        this.saveStudentConcession = function (SaveFeePaymentModel) {
            var response = $http({
                method: "post",
                url: "/api/StudentConcession/saveStudentConcession",
                data: JSON.stringify(SaveFeePaymentModel),
                dataType: "json",
            });
            return response;
        }



        //------------------------------------Fee Payment Cancle----------------------------------------
        this.getFeeCollectionForDelete = function (ReceiptNo) {
            return $http.get("/api/feePayment/FeeCollectionForDelete", { params: { ReceiptNo: ReceiptNo } });
        }
        this.deleteFeeCollection = function (FeeCollectionId) {
            return $http.get("/api/feePayment/DeleteFeeCollection", { params: { FeeCollectionId: FeeCollectionId } });
        }


        //------------------------------------Admission No Update----------------------------------------
        this.getAdmissionListForUpdateAdmisionNo = function (AcademicSessionId, ClassId, SectionId) {
            if (AcademicSessionId == null)
                AcademicSessionId = 0;
            return $http.get("/api/admissionUpdate/admissionList", { params: { AcademicSessionId: AcademicSessionId, ClassId: ClassId, SectionId: SectionId } });
        }

        this.updateAdmissionNos = function (admissionList) {
            var response = $http({
                method: "post",
                url: "/api/admissionUpdate/updateAdmissionNos",
                data: JSON.stringify(admissionList),
                dataType: "json",
            });
            return response;
        }

        this.updateStudentRollNo = function (admissionList) {
            var response = $http({
                method: "post",
                url: "/api/admissionUpdate/UpdateStudentRollNo",
                data: JSON.stringify(admissionList),
                dataType: "json",
            });
            return response;
        }


        this.updateAllStudentDetails = function (admissionList) {
            var response = $http({
                method: "post",
                url: "/api/admissionUpdate/updateStudentDetails",
                data: JSON.stringify(admissionList),
                dataType: "json",
            });
            return response;
        }


        //------------------------------------Miscellaneous Fee----------------------------------------
        this.checkAdmissionNoForMiscellaneousFee = function (AdmissionNo) {
            return $http.get("/api/miscellaneousFee/checkAdmissionNo", { params: { AdmissionNo: AdmissionNo } });
        }

        this.saveMiscellaneousFee = function (saveMiscellaneousFeeModel) {
            var response = $http({
                method: "post",
                url: "/api/miscellaneousFee/saveMiscellaneousFee",
                data: JSON.stringify(saveMiscellaneousFeeModel),
                dataType: "json",
            });
            return response;
        }


        //------------------------------------Student Registration --------------------------------------\\
        this.saveStudentRegistration = function (saveStudentRegistrationModel) {
            var response = $http({
                method: "post",
                url: "/api/StudentRegistration/saveStudentRegistration",
                data: JSON.stringify(saveStudentRegistrationModel),
                dataType: "json",
            });
            return response;
        }
        this.updateStudentRegistration = function (StudentRegistrationModel) {
            var response = $http({
                method: "post",
                url: "/api/StudentRegistration/UpdateStudentRegistration",
                data: JSON.stringify(StudentRegistrationModel),
                dataType: "json",
            });
            return response;
        }

        this.getStudentRegistrationList = function (AcademicSessionId) {
            return $http.get("/api/StudentRegistration/StudentRegistrationList", { params: { AcademicSessionId: AcademicSessionId } });
        }

        this.getStudentRegistrationListForSearch = function (RegistrationStatus) {
            return $http.get("/api/StudentRegistration/StudentRegistrationListForSearch", { params: { RegistrationStatus: RegistrationStatus } });
        }


        //------------------------------------Registration Fee Head--------------------------------------\\
        this.getRegistrationFeeHeadList = function (SaveRegistrationFeeHeadModel) {
            var response = $http({
                method: "post",
                url: "/api/RegistrationFeeHead/RegistrationFeeHeadList",
                data: JSON.stringify(SaveRegistrationFeeHeadModel),
                dataType: "json",
            });
            return response;
        }

        this.saveRegistrationFeeHead = function (SaveRegistrationFeeHeadModel) {
            var response = $http({
                method: "post",
                url: "/api/RegistrationFeeHead/saveRegistrationFeeHead",
                data: JSON.stringify(SaveRegistrationFeeHeadModel),
                dataType: "json",
            });
            return response;
        }



        //------------------------------------Registration Fee --------------------------------------\\
        this.getRegistrationFeeList = function (RegistrationFeeModel) {
            var response = $http({
                method: "post",
                url: "/api/RegistrationFee/RegistrationFeeList",
                data: JSON.stringify(RegistrationFeeModel),
                dataType: "json",
            });
            return response;
        }
        this.saveRegistrationFee = function (MonthList) {
            var response = $http({
                method: "post",
                url: "/api/RegistrationFee/saveRegistrationFee",
                data: JSON.stringify(MonthList),
                dataType: "json",
            });
            return response;
        }


        //------------------------------------SubjectCombination Fee --------------------------------------\\
        this.getSubjectCombinationFeeList = function (ClassId, AcademicSessionId) {
            return $http.get("/api/SubjectCombinationFee/SubjectCombinationFeeList", { params: { ClassId: ClassId, AcademicSessionId: AcademicSessionId } });
        }

        this.saveSubjectCombinationFee = function (SubjectCombinationList) {
            var response = $http({
                method: "post",
                url: "/api/SubjectCombinationFee/saveSubjectCombinationFee",
                data: JSON.stringify(SubjectCombinationList),
                dataType: "json",
            });
            return response;
        }


        //------------------------------------Admission Fee Head--------------------------------------\\
        this.getAdmissionFeeHeadList = function (SaveAdmissionFeeHeadModel) {
            var response = $http({
                method: "post",
                url: "/api/AdmissionFeeHead/AdmissionFeeHeadList",
                data: JSON.stringify(SaveAdmissionFeeHeadModel),
                dataType: "json",
            });
            return response;
        }

        this.saveAdmissionFeeHead = function (SaveAdmissionFeeHeadModel) {
            var response = $http({
                method: "post",
                url: "/api/AdmissionFeeHead/saveAdmissionFeeHead",
                data: JSON.stringify(SaveAdmissionFeeHeadModel),
                dataType: "json",
            });
            return response;
        }



        //------------------------------------Admission Fee --------------------------------------\\
        this.getAdmissionFeeList = function (AdmissionFeeModel) {
            var response = $http({
                method: "post",
                url: "/api/AdmissionFee/AdmissionFeeList",
                data: JSON.stringify(AdmissionFeeModel),
                dataType: "json",
            });
            return response;
        }
        this.saveAdmissionFee = function (MonthList) {
            var response = $http({
                method: "post",
                url: "/api/AdmissionFee/saveAdmissionFee",
                data: JSON.stringify(MonthList),
                dataType: "json",
            });
            return response;
        }

        this.getRegistrationFeeListForRegistration = function (ClassId, StudentTypeId, AcademicSessionId) {
            return $http.get("/api/RegistrationFee/RegistrationFeeListForRegistration", { params: { ClassId: ClassId, StudentTypeId: StudentTypeId, AcademicSessionId: AcademicSessionId } });
        }

        ////======================================Class Fine Head======================================
        this.getClassFineHeadList = function () {
            return $http.get("/api/ClassFineHead/ClassFineHeadList");
        }


        //------------------------------------Student Charge--------------------------
        this.saveStudentCharge = function (StudentChargeModel) {
            var response = $http({
                method: "post",
                url: "/api/StudentCharge/SaveStudentCharge",
                data: JSON.stringify(StudentChargeModel),
                dataType: "json",
            });
            return response;
        }

        this.getStudentChargeList = function (AdmissionId) {
            return $http.get("/api/StudentCharge/StudentChargeList", { params: { AdmissionId: AdmissionId } });
        }

        this.deleteStudentCharge = function (StudentChargeId) {
            return $http.get("/api/StudentCharge/DeleteStudentCharge", { params: { StudentChargeId: StudentChargeId } });
        }


        //------------------------------------Student Charge--------------------------
        this.saveVehicleFuelConsumption = function (VehicleFuelConsumptionModel) {
            var response = $http({
                method: "post",
                url: "/api/VehicleFuelConsumption/SaveVehicleFuelConsumption",
                data: JSON.stringify(VehicleFuelConsumptionModel),
                dataType: "json",
            });
            return response;
        }
        this.getVehicleFuelConsumptionList = function (dateFileterModel) {
            var response = $http({
                method: "post",
                url: "/api/VehicleFuelConsumption/VehicleFuelConsumptionList",
                data: JSON.stringify(dateFileterModel),
                dataType: "json",
            });
            return response;
        }


        this.deleteVehicleFuelConsumption = function (VehicleFuelConsumptionId) {
            return $http.get("/api/VehicleFuelConsumption/DeleteVehicleFuelConsumption", { params: { VehicleFuelConsumptionId: VehicleFuelConsumptionId } });
        }


        //------------------------------------Student Charge--------------------------
        this.saveStudentRemark = function (StudentRemarkModel) {
            var response = $http({
                method: "post",
                url: "/api/StudentRemark/SaveStudentRemark",
                data: JSON.stringify(StudentRemarkModel),
                dataType: "json",
            });
            return response;
        }

        this.getStudentRemarkList = function (StudentId) {
            return $http.get("/api/StudentRemark/StudentRemarkList", { params: { StudentId: StudentId } });
        }

        this.deleteStudentRemark = function (StudentRemarkId) {
            return $http.get("/api/StudentRemark/DeleteStudentRemark", { params: { StudentRemarkId: StudentRemarkId } });
        }


        //------------------------------------Student Subject Combination--------------------------
        this.saveStudentSubjectCombination = function (StudentSubjectCombinationModel) {
            var response = $http({
                method: "post",
                url: "/api/StudentSubjectCombination/SaveStudentSubjectCombination",
                data: JSON.stringify(StudentSubjectCombinationModel),
                dataType: "json",
            });
            return response;
        }

        this.getStudentSubjectCombinationList = function (AdmissionId) {
            return $http.get("/api/StudentSubjectCombination/StudentSubjectCombinationList", { params: { AdmissionId: AdmissionId } });
        }

        this.deleteStudentSubjectCombination = function (StudentSubjectCombinationId) {
            return $http.get("/api/StudentSubjectCombination/DeleteStudentSubjectCombination", { params: { StudentSubjectCombinationId: StudentSubjectCombinationId } });
        }

        //------------------------------------Subject Combination--------------------------
        this.saveSubjectCombination = function (SubjectCombinationModel) {
            var response = $http({
                method: "post",
                url: "/api/SubjectCombination/SaveSubjectCombination",
                data: JSON.stringify(SubjectCombinationModel),
                dataType: "json",
            });
            return response;
        }

        this.getSubjectCombinationList = function (AdmissionId) {
            return $http.get("/api/SubjectCombination/SubjectCombinationList", { params: { AdmissionId: AdmissionId } });
        }

        this.deleteSubjectCombination = function (SubjectCombinationId) {
            return $http.get("/api/SubjectCombination/DeleteSubjectCombination", { params: { SubjectCombinationId: SubjectCombinationId } });
        }

        //-----------------------Reports----------------------------------------------------------
        this.getDailyCollectionReport = function (DCRRequest) {
            var response = $http({
                method: "post",
                url: "/api/report/DailyCollectionReport",
                data: JSON.stringify(DCRRequest),
                dataType: "json",
            });
            return response;
        }

        this.getDailyConcessionReport = function (DCRRequest) {
            var response = $http({
                method: "post",
                url: "/api/report/DailyConcessionReport",
                data: JSON.stringify(DCRRequest),
                dataType: "json",
            });
            return response;
        }

        this.getDailyCollectionReportStudentwise = function (DCRRequest) {
            var response = $http({
                method: "post",
                url: "/api/report/DailyCollectionReportStudentwise",
                data: JSON.stringify(DCRRequest),
                dataType: "json",
            });
            return response;
        }
        this.getDailyConcessionReportStudentwise = function (DCRRequest) {
            var response = $http({
                method: "post",
                url: "/api/report/DailyConcessionReportStudentwise",
                data: JSON.stringify(DCRRequest),
                dataType: "json",
            });
            return response;
        }

        this.getDailyCollectionReportStudentwise2 = function (DCRRequest) {
            var response = $http({
                method: "post",
                url: "/api/report/DailyCollectionReportStudentwise2",
                data: JSON.stringify(DCRRequest),
                dataType: "json",
            });
            return response;
        }

        this.getFeeDueReport = function (obj) {
            return $http.post("/api/report/FeeDueReport", obj);
        }

        this.getFeeDueReportByAdmissionId = function (AdmissionId, UptoMonthNo, UpToYear, UpToMonthDataNo) {
            if (UpToYear == null)
                UpToYear = 0;
            if (UpToMonthDataNo == null)
                UpToMonthDataNo = 0;
            return $http.get("/api/report/FeeDueReport", { params: { AdmissionId: AdmissionId, UptoMonthNo: UptoMonthNo, UpToYear: UpToYear, UpToMonthDataNo: UpToMonthDataNo, UpToMonthDataNo: UpToMonthDataNo } });
        }

        this.getFeeDueReport2 = function (obj) {
            return $http.post("/api/report/FeeDueReport2", obj);
        }

        this.getTuitionFeeDueList = function (SectionId, UptoMonthNo, AcademicSessionId) {
            return $http.get("/api/report/TuitionFeeDueList", { params: { SectionId: SectionId, UptoMonthNo: UptoMonthNo, AcademicSessionId: AcademicSessionId } });
        }

        //----------------------------Fee Head----------------------------------------------
        this.getFeeHeadList = function () {
            return $http.get("/api/feeHead/FeeHeadList");
        }

        //-------------------------System Login----------------------------------------------
        this.getSystemLoginList = function () {
            return $http.get("/api/systemLogin/SystemLoginList");
        }
        this.logoutSystemLogin = function () {
            return $http.get("/api/systemLogin/Logout");
        }
        this.checkSysytemLogin = function () {
            return $http.get("/api/systemLogin/checkSysytemLogin");
        }
        this.changeSystemLoginPassword = function (ChangePasswordModel) {
            var response = $http({
                method: "post",
                url: "/api/systemLogin/changePassword",
                data: JSON.stringify(ChangePasswordModel),
                dataType: "json",
            });
            return response;
        }


        //----------------------------Student Promotion--------------------------------------\\
        this.getStudentPromatableList = function (CurrentSectionId, CurrentSessionId) {
            return $http.get("/api/ClassPromotion/StudentPromatableList", { params: { CurrentSectionId: CurrentSectionId, CurrentSessionId: CurrentSessionId } });
        }

        this.veriyfyClassPromotion = function (SaveClassPromotionModel) {
            var response = $http({
                method: "post",
                url: "/api/ClassPromotion/VeriyfyClassPromotion",
                data: JSON.stringify(SaveClassPromotionModel),
                dataType: "json",
            });
            return response;
        }

        this.SaveClassPromotion = function (SaveClassPromotionModel) {
            var response = $http({
                method: "post",
                url: "/api/ClassPromotion/SaveClassPromotion",
                data: JSON.stringify(SaveClassPromotionModel),
                dataType: "json",
            });
            return response;
        }

        //-------------------------Admission------------------------------------------\\
        this.newAdmission = function (admissionModel) {
            var response = $http({
                method: "post",
                url: "/api/admission/newAdmission",
                data: JSON.stringify(admissionModel),
                dataType: "json",
            });
            return response;
        }
        this.updateAdmission = function (admissionModel) {
            var response = $http({
                method: "post",
                url: "/api/admission/updateAdmission",
                data: JSON.stringify(admissionModel),
                dataType: "json",
            });
            return response;
        }

        this.getAdmissionList = function (AcademicSessionId, ClassId, SectionId, AdmissionFrom) {
            if (AdmissionFrom == null || AdmissionFrom == undefined)
                AdmissionFrom = 0;
            if (ClassId == null || ClassId == undefined)
                ClassId = 0;
            if (AcademicSessionId == null || AcademicSessionId == undefined)
                AcademicSessionId = 0;
            return $http.get("/api/admission/admissionList", { params: { AcademicSessionId: AcademicSessionId, ClassId: ClassId, SectionId: SectionId, AdmissionFrom: AdmissionFrom } });
        }

        this.getAdmissionListForLibrary = function (AcademicSessionId, ClassNo, SectionId) {
            if (AcademicSessionId == null || AcademicSessionId == undefined)
                AcademicSessionId = 0;
            if (ClassNo == null || ClassNo == undefined)
                ClassNo = 0;
            if (SectionId == null || SectionId == undefined)
                SectionId = 0;
            return $http.get("/api/admission/AdmissionListForLibrary", { params: { AcademicSessionId: AcademicSessionId, ClassNo: ClassNo, SectionId: SectionId } });
        }

        this.getNewAdmissionList = function (AcademicSessionId, FromDate, ToDate) {
            return $http.get("/api/admission/admissionList", { params: { AcademicSessionId: AcademicSessionId, FromDate: FromDate, ToDate: ToDate } });
        }


        this.getStudentAdmissionList = function (StudentId) {
            return $http.get("/api/admission/StudentAdmissionList", { params: { StudentId: StudentId } });
        }

        this.getSearchAdmissionList = function () {
            return $http.get("/api/admission/SearchAdmissionList");
        }

        this.getAdmissionDetail = function (AdmissionId) {
            return $http.get("/api/admission/AdmissionDetail", { params: { AdmissionId: AdmissionId } });
        }


        //-----------------------------Fee Payment----------------------------------------------
        this.getFeePaymentDetail = function (AdmissionId) {
            return $http.get("/api/feePayment/feePaymentDetail", { params: { AdmissionId: AdmissionId } });
        }
        this.getFeePaymentListByAdmissionId = function (AdmissionId, NoOfRecords) {
            return $http.get("/api/feePayment/FeePaymentListByAdmissionId", { params: { AdmissionId: AdmissionId, NoOfRecords: NoOfRecords } });
        }
        this.getFeePaymentListByStudentId = function (StudentId, NoOfRecords) {
            return $http.get("/api/feePayment/FeePaymentListByStudentId", { params: { StudentId: StudentId, NoOfRecords: NoOfRecords } });
        }
        this.saveFeePayment = function (saveFeePaymentModel) {
            var response = $http({
                method: "post",
                url: "/api/feePayment/saveFeePayment",
                data: JSON.stringify(saveFeePaymentModel),
                dataType: "json",
            });
            return response;
        }
        this.updateFeePayment = function (FeeCollectionModel) {
            var response = $http({
                method: "post",
                url: "/api/feePayment/updateFeePayment",
                data: JSON.stringify(FeeCollectionModel),
                dataType: "json",
            });
            return response;
        }

        //------------------------------------Class Fee Head--------------------------------------\\
        this.getClassFeeHeadList = function (ClassFeeHeadModel) {
            var response = $http({
                method: "post",
                url: "/api/classFeeHead/ClassFeeHeadList",
                data: JSON.stringify(ClassFeeHeadModel),
                dataType: "json",
            });
            return response;
        }
        this.saveClassFeeHead = function (MonthList) {
            var response = $http({
                method: "post",
                url: "/api/classFeeHead/saveClassFeeHead",
                data: JSON.stringify(MonthList),
                dataType: "json",
            });
            return response;
        }



        //------------------------------------Class Fee --------------------------------------\\
        this.getClassFeeList = function (ClassFeeModel) {
            var response = $http({
                method: "post",
                url: "/api/ClassFee/ClassFeeList",
                data: JSON.stringify(ClassFeeModel),
                dataType: "json",
            });
            return response;
        }
        this.saveClassFee = function (MonthList) {
            var response = $http({
                method: "post",
                url: "/api/ClassFee/saveClassFee",
                data: JSON.stringify(MonthList),
                dataType: "json",
            });
            return response;
        }



        //-------------------------Student------------------------------------------\\
        this.updateStudent = function (studentModel) {
            var response = $http({
                method: "post",
                url: "/api/student/UpdateStudent",
                data: JSON.stringify(studentModel),
                dataType: "json",
            });
            return response;
        }

        this.uploadStudentPhoto = function (data) {
            return $http.post('/api/student/updateStudentPhoto', data, {
                transformRequest: angular.identity,
                headers: { 'Content-Type': undefined }
            })
        }

        this.GenerateAdmissionNo = function (AcademicSessionId, SectionId) {
            return $http.get("/api/student/GenerateAdmissionNo", { params: { AcademicSessionId: AcademicSessionId, SectionId: SectionId } });
        }


        this.getSearchStudentList = function (status) {
            return $http.get("/api/student/SearchStudentList", { params: { status: status } });
        }

        //---------------------------Class----------------------------------------\\
        this.getClassList = function (ClassStatus) {
            return $http.get("/api/class/classlist", { params: { ClassStatus: ClassStatus } });
        }

        this.getClassListDistinct = function (ClassStatus) {
            return $http.get("/api/class/classListDistinct", { params: { ClassStatus: ClassStatus } });
        }

        this.getClassListDistinctForClassTeacher = function (ClassStatus, AcademicSessionId) {
            if (AcademicSessionId == null || AcademicSessionId == undefined)
                AcademicSessionId = 0;
            return $http.get("/api/class/ClassListDistinctForClassTeacher", { params: { ClassStatus: ClassStatus, AcademicSessionId: AcademicSessionId } });
        }

        this.getAvailableStudents = function (AcademicSessionId, SectionId) {
            return $http.get("/api/class/AvailableStudents", { params: { AcademicSessionId: AcademicSessionId, SectionId: SectionId } });
        }


        //---------------------------------Student Type-------------------------------------
        this.getStudentTypeList = function () {
            return $http.get("/api/admission/StudentTypeList");
        }


        //---------------------------------Semester--------------------------------------------
        this.getSemesterList = function (SemesterModel) {
            var response = $http({
                method: "post",
                url: "/api/Semester/SemesterList",
                data: JSON.stringify(SemesterModel),
                dataType: "json",
            });
            return response;
        };
        this.saveSemester = function (SemesterModel) {
            var response = $http({
                method: "post",
                url: "/api/Semester/SaveSemester",
                data: JSON.stringify(SemesterModel),
                dataType: "json",
            });
            return response;
        };
        this.deleteSemester = function (SemesterId) {
            return $http.get("/api/Semester/deleteSemester", { params: { SemesterId: SemesterId } });
        }


        //---------------------------------AcademicSession--------------------------------------------
        this.getAcademicSessionList = function (obj) {
            if (typeof (obj) != "object")
                obj = {};
            var response = $http({
                method: "post",
                url: "/api/AcademicSession/AcademicSessionList",
                data: JSON.stringify(obj),
                dataType: "json",
            });
            return response;
        };
        this.saveAcademicSession = function (AcademicSessionModel) {
            var response = $http({
                method: "post",
                url: "/api/AcademicSession/SaveAcademicSession",
                data: JSON.stringify(AcademicSessionModel),
                dataType: "json",
            });
            return response;
        };
        this.deleteAcademicSession = function (AcademicSessionId) {
            return $http.get("/api/AcademicSession/deleteAcademicSession", { params: { AcademicSessionId: AcademicSessionId } });
        }

        //---------------------------------Section--------------------------------------------
        this.getSectionList = function (SectionStatus, ClassId) {
            if (ClassId == null)
                ClassId = 0;
            return $http.get("/api/section/SectionList", { params: { SectionStatus: SectionStatus, ClassId: ClassId } });
        }
        this.getSectionListForClassTeacher = function (SectionStatus, ClassId, AcademicSessionId) {
            if (ClassId == null)
                ClassId = 0;
            if (AcademicSessionId == null || AcademicSessionId == undefined)
                AcademicSessionId = 0;
            return $http.get("/api/section/SectionListForClassTeacher", { params: { SectionStatus: SectionStatus, ClassId: ClassId, AcademicSessionId: AcademicSessionId } });
        }

        this.getSectionList2 = function (SectionStatus, ClassId) {
            if (ClassId == null)
                ClassId = 0;
            return $http.get("/api/section/SectionList2", { params: { SectionStatus: SectionStatus, ClassId: ClassId } });
        }

        this.saveSection = function (SectionModel) {
            var response = $http({
                method: "post",
                url: "/api/Section/SaveSection",
                data: JSON.stringify(SectionModel),
                dataType: "json",
            });
            return response;
        };
        this.deleteSection = function (SectionId) {
            return $http.get("/api/Section/deleteSection", { params: { SectionId: SectionId } });
        }

        this.getSectionPeriodList = function (AcademicSessionId) {
            return $http.get("/api/section/SectionPeriodList", { params: { AcademicSessionId: AcademicSessionId } });
        }

        //----------------------------------Supplier------------------------------------------------------
        this.getSupplierList = function (Status, SupplierType) {
            SupplierType = loadDataService.getInt(SupplierType);
            Status = loadDataService.getInt(Status);
            return $http.get("/api/Supplier/SupplierList", { params: { Status: Status, SupplierType: SupplierType } });
        }

        this.saveSupplier = function (SupplierModel) {
            var response = $http({
                method: "post",
                url: "/api/Supplier/SaveSupplier",
                data: JSON.stringify(SupplierModel),
                dataType: "json",
            });
            return response;
        };
        this.deleteSupplier = function (SupplierId) {
            return $http.get("/api/Supplier/deleteSupplier", { params: { SupplierId: SupplierId } });
        }

        //-------------------------------------House--------------------------------------------------
        this.getHouseList = function (HouseStatus) {
            return $http.get("/api/House/HouseList", { params: { HouseStatus: HouseStatus } });
        }


        //-------------------------------------State--------------------------------------------------
        this.getStateList = function (StateStatus) {
            return $http.get("/api/State/StateList", { params: { StateStatus: StateStatus } });
        }

        this.saveState = function (StateModel) {
            var response = $http({
                method: "post",
                url: "/api/state/SaveState",
                data: JSON.stringify(StateModel),
                dataType: "json",
            });
            return response;
        };

        this.deleteState = function (StateId) {
            return $http.get("/api/state/deleteState", { params: { StateId: StateId } });
        }

        //-------------------------------------City--------------------------------------------------
        this.getCityList = function (CityStatus) {
            return $http.get("/api/City/CityList", { params: { CityStatus: CityStatus } });
        }

        this.saveCity = function (CityModel) {
            var response = $http({
                method: "post",
                url: "/api/city/SaveCity",
                data: JSON.stringify(CityModel),
                dataType: "json",
            });
            return response;
        };

        this.deletecity = function (CityId) {
            return $http.get("/api/City/deleteCity", { params: { CityId: CityId } });
        }

        //-------------------------Transport start----------------------------------------------------
        this.getTransportFeePeriodList = function () {
            return $http.get("/api/transport/transportperiodList");
        }

        this.savePeriod = function (period) {
            var response = $http({
                method: "post",
                url: "/api/transport/saveTransportFeePeriod",
                data: JSON.stringify(period),
                dataType: "json",
            });
            return response;
        }

        this.getTransportGroupList = function () {
            return $http.get("/api/transport/transportGroupList");
        }

        this.saveGroup = function (group) {
            var response = $http({
                method: "post",
                url: "/api/transport/saveTransportGroup",
                data: JSON.stringify(group),
                dataType: "json",
            });
            return response;
        }

        this.getTransportFeeHeadList = function () {
            return $http.get("/api/transport/trabsportFeeHeadList")
        }

        this.feeHeadSave = function (head) {
            var response = $http({
                method: "post",
                url: "/api/transport/SavetrabsportFeeHead",
                data: JSON.stringify(head),
                dataType: "json",
            });
            return response;
        }

        this.transportFeeList = function (periodId) {
            return $http.get("/api/transport/transportFeeList", { params: { periodId: periodId } })
        }

        this.feeSave = function (model) {
            var response = $http({
                method: "post",
                url: "/api/transport/SavetrabsportFee",
                data: JSON.stringify(model),
                dataType: "json",
            });
            return response;
        }

        this.saveBus = function (bus) {
            var response = $http({
                method: "post",
                url: "/api/transport/saveBusDetails",
                data: JSON.stringify(bus),
                dataType: "json",
            });
            return response;
        }

        this.getBusList = function () {
            return $http.get("/api/transport/transportBusList");
        }

        this.getTransportList = function (admissionID) {
            return $http.get("/api/transport/studentTransportDetails", { params: { admissionId: admissionID } })
        }

        this.saveTransortDetails = function (details) {
            var response = $http({
                method: "post",
                url: "/api/transport/saveStudentTransport",
                data: JSON.stringify(details),
                dataType: "json",
            });
            return response;
        }

        this.StudentTransportReport = function (sessionId, ReportFor, classId, sectionId, groupId, busId) {
            return $http.get("/Admin/LoadStudentTransportReport", { params: { sessionId: sessionId, ReportFor: ReportFor, classId: classId, sectionId: sectionId, groupId: groupId, busId: busId } })
        }
        //-------------------------Transport End------------------------------------------------------




        /////////////////////////////////////Student Document////////////////////////////////////////////

        this.Studentlist = function () {
            return $http.get("/api/StudentDocument/Studentlist")
        }
        this.saveDocument = function (Documentad) {
            var response = $http({
                method: "post",
                url:"/api/StudentDocument/SaveDocument",
                data: JSON.stringify(Documentad),
                dataType:"Json",
            });
            return response;

        }
        this.DocumentList = function () {
            return $http.get("/api/StudentDocument/StudentDocumentList")
        }

        this.deleteDocument = function (DocumentID) {
            return $http.get("/api/StudentDocument/deleteDocument", { params: { DocumentID: DocumentID } });
        }

        //-------------------------Hostel start----------------------------------------------------
        this.getHostelFeePeriodList = function () {
            return $http.get("/api/Hostel/HostelperiodList");
        }

        this.savePeriod = function (period) {
            var response = $http({
                method: "post",
                url: "/api/Hostel/saveHostelFeePeriod",
                data: JSON.stringify(period),
                dataType: "json",
            });
            return response;
        }

        this.getHostelList = function () {
            return $http.get("/api/Hostel/HostelList");
        }

        this.saveHostel = function (Hostel) {
            var response = $http({
                method: "post",
                url: "/api/Hostel/saveHostel",
                data: JSON.stringify(Hostel),
                dataType: "json",
            });
            return response;
        }

        this.getHostelFeeHeadList = function () {
            return $http.get("/api/Hostel/HostelFeeHeadList")
        }

        this.saveHostelFeeHead = function (head) {
            var response = $http({
                method: "post",
                url: "/api/Hostel/SaveHostelFeeHead",
                data: JSON.stringify(head),
                dataType: "json",
            });
            return response;
        }

        this.HostelFeeList = function (periodId) {
            return $http.get("/api/Hostel/HostelFeeList", { params: { periodId: periodId } })
        }

        this.saveHostelFee = function (model) {
            var response = $http({
                method: "post",
                url: "/api/Hostel/SaveHostelFee",
                data: JSON.stringify(model),
                dataType: "json",
            });
            return response;
        }


        this.getStudnetHostelList = function (admissionID) {
            return $http.get("/api/Hostel/studentHostelDetails", { params: { admissionId: admissionID } })
        }

        this.saveStudentHostel = function (details) {
            var response = $http({
                method: "post",
                url: "/api/Hostel/saveStudentHostel",
                data: JSON.stringify(details),
                dataType: "json",
            });
            return response;
        }

        this.StudentHostelReport = function (sessionId, ReportFor, classId, sectionId, HostelId, busId) {
            return $http.get("/Admin/LoadStudentHostelReport", { params: { sessionId: sessionId, ReportFor: ReportFor, classId: classId, sectionId: sectionId, HostelId: HostelId, busId: busId } })
        }
        //-------------------------Hostel End------------------------------------------------------

        //----------------------------------------------------Performance----------------------------------------------------
        this.saveTeacherPerformance = function (obj) {
            var response = $http({
                method: "post",
                url: "/api/Performance/SaveTeacherPerformance",
                data: JSON.stringify(obj),
                dataType: "json",
            });
            return response;
        };
        this.saveTeacherPerformanceByPrincipal = function (obj) {
            var response = $http({
                method: "post",
                url: "/api/Performance/SaveTeacherPerformanceByPrincipal",
                data: JSON.stringify(obj),
                dataType: "json",
            });
            return response;
        };

        this.getParentPerformanceList = function (obj) {
            var response = $http({
                method: "post",
                url: "/api/Performance/ParentPerformanceList",
                data: JSON.stringify(obj),
                dataType: "json",
            });
            return response;
        };

        this.getStudentPerformanceList = function (obj) {
            var response = $http({
                method: "post",
                url: "/api/Performance/StudentPerformanceList",
                data: JSON.stringify(obj),
                dataType: "json",
            });
            return response;
        };

        this.getTeacherPerformanceList = function (obj) {
            var response = $http({
                method: "post",
                url: "/api/Performance/TeacherPerformanceList",
                data: JSON.stringify(obj),
                dataType: "json",
            });
            return response;
        };

        this.getTeacherFeedbackList = function (obj) {
            var response = $http({
                method: "post",
                url: "/api/Performance/TeacherFeedbackList",
                data: JSON.stringify(obj),
                dataType: "json",
            });
            return response;
        };

        this.getTeacherAttitudeList = function (obj) {
            var response = $http({
                method: "post",
                url: "/api/Performance/TeacherAttitudeList",
                data: JSON.stringify(obj),
                dataType: "json",
            });
            return response;
        };

        this.getPerformanceYearList = function () {
            return $http.get("/api/Performance/PerformanceYearList");
        }

        this.getTeachingLearningAidsDetail = function (AcademicSessionId) {
            return $http.get("/api/Performance/TeachingLearningAidsDetail", { params: { AcademicSessionId: AcademicSessionId } });
        }

        this.getTeacherParameterList = function () {
            return $http.get("/api/Performance/TeacherParameterList");
        }

        this.getTeacherSubjectListForFeedback = function (AcademicSessionId, TeacherId) {
            if (TeacherId == null || TeacherId == undefined)
                TeacherId = 0;
            return $http.get("/api/Performance/TeacherSubjectListForFeedback", { params: { AcademicSessionId: AcademicSessionId, TeacherId: TeacherId } });
        }

        //-----------------------------------------------Performance Parameter----------------------------------------------
        this.getParentPerformanceParameterList = function (PerformanceId) {
            return $http.get("/api/PerformanceParameter/ParentPerformanceParameterList", { params: { PerformanceId: PerformanceId } });
        }
        this.getTeacherPerformanceParameterList = function (PerformanceId) {
            return $http.get("/api/PerformanceParameter/TeacherPerformanceParameterList", { params: { PerformanceId: PerformanceId } });
        }
        this.getTeacherFeedbackListFromPricipal = function (PerformanceId) {
            return $http.get("/api/PerformanceParameter/TeacherFeedbackListFromPricipal", { params: { PerformanceId: PerformanceId } });
        }
        this.getStudentPerformanceParameterList = function (PerformanceId) {
            return $http.get("/api/PerformanceParameter/StudentPerformanceParameterList", { params: { PerformanceId: PerformanceId } });
        }

    }
}());