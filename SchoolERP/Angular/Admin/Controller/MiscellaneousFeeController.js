(function () {
    'use strict';

    angular
        .module('app')
        .controller('MiscellaneousFeeController', MiscellaneousFeeController);

    MiscellaneousFeeController.$inject = ['$scope', 'adminService', 'ConstantData', 'loadDataService', 'Factory'];

    function MiscellaneousFeeController($scope, adminService, ConstantData, loadDataService, Factory) {
        $scope.checkSysytemLogin();
        $scope.Admission = {};
        $scope.Admission.ClassId = null;
        $scope.Admission.SectionId = null;
        $scope.FeeCollection = {};
        $scope.FeeCollection.PaymentDate = new Date();
        $scope.FeeCollection.PaymentMode = null;
        $scope.FeeCollection.DemandDraftDate = new Date();
        $scope.FeeCollection.ChequeDate = new Date();
        $scope.FeeTypeList = ConstantData.FeeType;
        $scope.PaymentModeList = ConstantData.PaymentMode;
        $scope.PaymentModes = ConstantData.PaymentModes;

        $scope.currentPage = 1;
        $scope.sizeOptions = Factory.SizeOptions;
        $scope.pageSize = $scope.sizeOptions[1];

        $scope.$watch('formFeeCollection.$valid', function (value) {
            $scope.IsFormValid = value;
        })

        $scope.$watch('formAdmission.$valid', function (value) {
            $scope.IsFormAdmissionValid = value;
        })

        $scope.getClassList = function () {
            $scope.dataLoading = true;
            adminService.getClassList(ConstantData.ClassStatus[1].Key)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.ClassList = response.data.ClassList;
                        var index = { ClassId: null, ClassName: '--Select Class--' };
                        $scope.ClassList.unshift(index);
                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                }, function (err) {
                    toastr.error("Some error occured while fetching data");
                    $scope.dataLoading = false;
                })
        }
        $scope.getClassList();

        $scope.getFeeHeadList = function () {
            $scope.dataLoading = true;
            adminService.getFeeHeadList()
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.FeeHeads = response.data.FeeHeadList;
                        var index = { FeeHeadId: null, FeeHeadTitle: '--Select Fee Head--' };
                        $scope.FeeHeads.unshift(index);
                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                }, function (err) {
                    toastr.error("Some error occured while fetching data");
                    $scope.dataLoading = false;
                })
        }
        $scope.getFeeHeadList();

        $scope.changeClass = function () {
            $scope.SectionList = [];
            angular.forEach($scope.MainSectionList, function (section, key) {
                if (section.ClassId == $scope.Admission.ClassId) {
                    $scope.SectionList.push(section);
                }
            })
            var index = { SectionId: null, SectionName: '--Select Section--' };
            $scope.SectionList.unshift(index);
        }
        $scope.changeClass();

        $scope.getSectionList = function () {
            $scope.dataLoading = true;
            adminService.getSectionList(ConstantData.SectionStatus[1].Key)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.MainSectionList = response.data.SectionList;
                        $scope.changeClass();
                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                }, function (err) {
                    toastr.error("Some error occured while fetching data");
                    $scope.dataLoading = false;
                })
        }
        $scope.getSectionList();

        $scope.resetForm = function () {
            $scope.FeeCollection = {};
            $scope.FeeCollection.PaymentDate = new Date();
            $scope.FeeCollection.PaymentMode = null;
            $scope.FeeCollection.DemandDraftDate = new Date();
            $scope.FeeCollection.ChequeDate = new Date();
            $scope.IsSubmitted = false;
            $scope.formFeeCollection.$setPristine();
            $scope.formFeeCollection.$setUntouched();
            $scope.formAdmission.$setPristine();
            $scope.formAdmission.$setUntouched();
        }
        $scope.checkAdmissionNoForMiscellaneousFee = function () {
            if ($scope.Admission.AdmissionNo == null || $scope.Admission.AdmissionNo == undefined) {
                $scope.Admission.AdmissionNo = "";
            }
            console.log($scope.Admission);
            $scope.dataLoading = true;
            adminService.checkAdmissionNoForMiscellaneousFee($scope.Admission.AdmissionNo)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        //if (response.data.Miscellaneous.AdmissionNo != null) {
                        var AdmissionNo = $scope.Admission.AdmissionNo;
                        $scope.Admission = response.data.Miscellaneous;
                        if ($scope.Admission.AdmissionId == 0 && $scope.Admission.MiscellaneousId == 0) {
                            $scope.Admission.FeeCollectionList = [];
                        } else {
                            $scope.Admission.MobileNo = parseInt($scope.Admission.MobileNo);
                            $scope.changeClass();
                        }
                        if ($scope.Admission.AdmissionId == 0 && $scope.Admission.MiscellaneousId == 0) {
                            $scope.Admission.AdmissionNo = AdmissionNo;
                        }
                        //}
                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                }, function (err) {
                    toastr.error("Some error occured while fetching data");
                    $scope.dataLoading = false;
                })
        }

        $scope.FeeHeadId = null;
        $scope.FeeHeadList = [];
        $scope.addFeeHead = function () {
            if ($scope.FeeHeadId == null) {
                toastr.error("Invalid Fee Head!", "Validation Error");
                return;
            }
            if ($scope.Amount > 0) {
                angular.forEach($scope.FeeHeads, function (feeHead, key) {
                    if ($scope.FeeHeadId == feeHead.FeeHeadId) {
                        var FeeHead = {
                            FeeHeadTitle: feeHead.FeeHeadTitle,
                            FeeHeadId: $scope.FeeHeadId,
                            Amount: $scope.Amount
                        }
                        $scope.FeeHeadList.push(FeeHead);
                        $scope.Amount = null;
                        $scope.calculateFeeHeadTotal();
                    }
                })
            } else {
                toastr.error("Invalid Amount!!", "Validation Error");
            }
        }

        $scope.removeFeeHead = function (index) {
            $scope.FeeHeadList.splice(index, 1);
            $scope.calculateFeeHeadTotal();
        }

        $scope.calculateFeeHeadTotal = function () {
            $scope.FeeCollection.PaidAmount = 0;
            angular.forEach($scope.FeeHeadList, function (feehead, key) {
                $scope.FeeCollection.PaidAmount += feehead.Amount;
            })
        }


        $scope.saveMiscellaneousFee = function () {
            $scope.IsSubmitted = true;
            if (!$scope.IsFormValid) {
                toastr.error("Fill all required fields.")
                return;
            }
            if ($scope.Admission.AdmissionId == 0 && $scope.Admission.MiscellaneousId) {
                if (!$scope.IsFormAdmissionValid) {
                    toastr.error("Fill Student Detail required fields.")
                    return;
                }
            }
            var saveAdmissionModel = {
                Miscellaneous: {
                    AdmissionId: $scope.Admission.AdmissionId,
                    AdmissionNo: $scope.Admission.AdmissionNo,
                    MiscellaneousId: $scope.Admission.MiscellaneousId,
                    FullName: $scope.Admission.FullName,
                    FatherName: $scope.Admission.FatherName,
                    SectionId: $scope.Admission.SectionId,
                    MobileNo: $scope.Admission.MobileNo,
                    PaymentCategory: $scope.Admission.PaymentCategory,
                    AcademicSessionId: $scope.Admission.AcademicSessionId,
                    SchoolId: $scope.Admission.SchoolId,
                },
                FeeCollection: {
                    PaymentDate: loadDataService.getDateTime($scope.FeeCollection.PaymentDate),
                    PaymentMode: $scope.FeeCollection.PaymentMode,
                    PreviousDues: $scope.FeeCollection.PreviousDues,
                    LineTotal: $scope.FeeCollection.LineTotal,
                    Concession: $scope.FeeCollection.Concession,
                    Subtotal: $scope.FeeCollection.Subtotal,
                    PaidAmount: $scope.FeeCollection.PaidAmount,
                    DuesAmount: $scope.FeeCollection.DuesAmount,
                    Remarks: $scope.FeeCollection.Remarks,
                    RemarksForConcession: $scope.FeeCollection.RemarksForConcession,
                    ChequeNo: $scope.FeeCollection.ChequeNo,
                    ChequeDate: loadDataService.getDateTime($scope.FeeCollection.ChequeDate),
                    ChequeBank: $scope.FeeCollection.ChequeBank,
                    ChequeBankBranch: $scope.FeeCollection.ChequeBankBranch,
                    DemandDraftNo: $scope.FeeCollection.DemandDraftNo,
                    DemandDraftDate: loadDataService.getDateTime($scope.FeeCollection.DemandDraftDate),
                    DemandDraftBank: $scope.FeeCollection.DemandDraftBank,
                    TransactionNo: $scope.FeeCollection.TransactionNo,
                },
                FeeHeadList: $scope.FeeHeadList

            }
            $scope.dataLoading = true;
            adminService.saveMiscellaneousFee(saveAdmissionModel)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.checkAdmissionNoForMiscellaneousFee();
                        toastr.success("Payment Taken Successfully. Receipt No: " + response.data.ReceiptNo);
                        if (response.data.MiscellaneousId > 0) {
                            window.open('/admin/PrintMiscellaneousFeeReceipt/' + response.data.FeeCollectionId);
                        } else {
                            window.open('/admin/PrintCaseFeeReceipt/' + response.data.FeeCollectionId);
                        }
                        $scope.resetForm();
                        $scope.FeeHeadList = [];
                    } else {
                        toastr.error(response.data.Message);
                        $scope.dataLoading = false;
                    }
                }, function (err) {
                    toastr.error("Some error occured while fetching data");
                    $scope.dataLoading = false;
                })
        }
    }
})();
