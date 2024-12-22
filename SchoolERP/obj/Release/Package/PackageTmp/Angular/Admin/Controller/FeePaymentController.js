(function () {
    'use strict';

    angular
        .module('app')
        .controller('FeePaymentController', FeePaymentController);

    FeePaymentController.$inject = ['$scope', 'adminService', 'ConstantData', 'loadDataService', 'Factory'];

    function FeePaymentController($scope, adminService, ConstantData, loadDataService, Factory) {
        $scope.checkSysytemLogin();
        $scope.MonthFeeList = [];
        $scope.FeeCollection = {};
        $scope.FeeCollection.PaymentDate = new Date();
        $scope.FeeCollection.PaymentMode = null;
        $scope.FeeCollection.DemandDraftDate = new Date();
        $scope.FeeCollection.ChequeDate = new Date();
        $scope.FeeHeadGroupList = [];
        $scope.FeeCollectionList = [];
        $scope.FeeTypeList = ConstantData.FeeType;
        $scope.PaymentModeList = ConstantData.PaymentMode;
        $scope.PaymentModes = ConstantData.PaymentModes;

        var todayDate = new Date();
        $scope.currentYear = todayDate.getFullYear();

        $scope.$watch('formFeeCollection.$valid', function (value) {
            $scope.IsFormValid = value;
        })

        $scope.resetForm = function () {
            $scope.MonthFeeList = [];
            $scope.YearFeeList = [];
            $scope.FeeCollection = {};
            $scope.FeeCollection.PaymentDate = new Date();
            $scope.FeeCollection.PaymentMode = null;
            $scope.FeeCollection.DemandDraftDate = new Date();
            $scope.FeeCollection.ChequeDate = new Date();
            $scope.FeeHeadGroupList = [];
            $scope.SelectedAdmission = {};
            $scope.Admission = {};
            $scope.AdmissionId = 0;
            $scope.IsSubmitted = false;
            $scope.$broadcast('angucomplete-alt:clearInput', 'StudentAutoComplete');
            $scope.formFeeCollection.$setPristine();
            $scope.formFeeCollection.$setUntouched();
        }

        $scope.getSearchAdmissionList = function () {
            $scope.dataLoading = true;
            adminService.getSearchAdmissionList()
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.AdmissionList = response.data.AdmissionList;
                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                }, function (err) {
                    toastr.error("Some error occured while fetching data");
                    $scope.dataLoading = false;
                })
        }
        $scope.getSearchAdmissionList();

        $scope.currentPage = 1;
        $scope.sizeOptions = Factory.SizeOptions;
        $scope.pageSize = $scope.sizeOptions[1];
        $scope.sort = function (key) {
            $scope.sortKey = key;
            $scope.reverse = !$scope.reverse;
        };
        $scope.getFeePaymentListByAdmissionId = function (AdmissionId) {
            $scope.dataLoading = true;
            adminService.getFeePaymentListByAdmissionId(AdmissionId, 0)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.FeeCollectionList = response.data.FeeCollectionList;
                    } else {
                        toastr.error(response.data.Message);
                    }
                }, function (err) {
                    toastr.error("Some error occured while fetching data");
                    $scope.dataLoading = false;
                })
        }
        $scope.getFeePaymentListByStudentId = function (StudentId) {
            $scope.dataLoading = true;
            adminService.getFeePaymentListByStudentId(StudentId, 0)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.AllFeeCollectionList = response.data.FeeCollectionList;
                        $('#modal_fee_collection_list').modal('show');
                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                }, function (err) {
                    toastr.error("Some error occured while fetching data");
                    $scope.dataLoading = false;
                })
        }

        $scope.getFeePaymentDetail = function (AdmissionId) {
            $scope.dataLoading = true;
            adminService.getFeePaymentDetail(AdmissionId)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        //console.log(response.data);
                        $scope.FeeCollection.PreviousDueRemarks = response.data.PreviousDueRemarks;
                        $scope.StudentChargeFeeHeadList = response.data.StudentChargeFeeHeadList;
                        $scope.FeeCollection.PreviousDues = response.data.PreviousDueAmount;
                        $scope.Admission = response.data.AdmissionDetail;
                        $scope.MonthFeeList = response.data.MonthFeeList;
                        $scope.YearFeeList = response.data.YearFeeList;
                        $scope.AdmissionId = $scope.Admission.AdmissionId;
                        $scope.PaidFeeHeadList = response.data.PaidFeeHeadList;
                        $scope.TuitionFeeHeadId = response.data.TuitionFeeHeadId;
                        $scope.calculateDueFeeHeadGroups();
                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                }, function (err) {
                    toastr.error("Some error occured while fetching data");
                    $scope.dataLoading = false;
                })
        }

        $scope.afterStudentSelected = function (selected) {
            if (selected != undefined) {
                $scope.StudentChargeFeeHeadList = [];
                $scope.MonthFeeList =[];
                $scope.YearFeeList = [];
                $scope.PaidFeeHeadList = [];
                $scope.SelectedAdmission = selected.originalObject;
                //if ($scope.AdmissionId != $scope.SelectedAdmission.AdmissionId) {
                $scope.getFeePaymentDetail($scope.SelectedAdmission.AdmissionId);
                $scope.getFeePaymentListByAdmissionId($scope.SelectedAdmission.AdmissionId);
                $scope.getStudentRemarkList($scope.SelectedAdmission.StudentId)
                //}
            } else {

            }
        }

        $scope.changeIsShow = function (model) {
            model.IsShow = !model.IsShow;
        }

        $scope.cahngeMonthFee = function (index, monthFeeModel) {
            if (monthFeeModel.IsChecked) {
                for (var i = index; i >= 0; i--) {
                    $scope.MonthFeeList[i].IsChecked = true;
                }
            }
            else {
                for (var i = index; i < $scope.MonthFeeList.length; i++) {
                    var monthFee = $scope.MonthFeeList[i];
                    monthFee.IsChecked = false;
                }
            }
            $scope.calculateFeeHeadGroups(true);
        }

        $scope.checkFeeHeadGroups = function (feeHead) {
            var IsAdded = false;
            for (var i = 0; i < $scope.FeeHeadGroupList.length; i++) {
                var feeHeadGroup = $scope.FeeHeadGroupList[i];
                if (feeHead.FeeHeadId == feeHeadGroup.FeeHeadId && feeHead.FeeType == feeHeadGroup.FeeType) {
                    feeHeadGroup.FeeAmount += feeHead.FeeAmount;
                    feeHeadGroup.Concession += feeHead.Concession;
                    feeHeadGroup.LineTotal = feeHeadGroup.FeeAmount - feeHeadGroup.Concession;
                    IsAdded = true;
                    break;
                }
            }
            if (!IsAdded) {
                if (feeHead.FeeAmount != 0) {
                    var newFeeHead = {
                        Concession: feeHead.Concession,
                        EnableEdit: feeHead.EnableEdit,
                        FeeAmount: feeHead.FeeAmount,
                        FeeHeadId: feeHead.FeeHeadId,
                        FeeHeadTitle: feeHead.FeeHeadTitle,
                        FeeType: feeHead.FeeType,
                        IsAdmissionFee: feeHead.IsAdmissionFee,
                        IsFine: feeHead.IsFine,
                        IsSelected: feeHead.IsSelected,
                        LineTotal: feeHead.LineTotal,
                        StudentChargeId: feeHead.StudentChargeId,
                    }
                    $scope.FeeHeadGroupList.push(newFeeHead);
                }
            }
        }

        $scope.calculateFeeHeadGroups = function (isReset) {
            if (isReset)
                $scope.FeeHeadGroupList = [];
            var lateFine = {
                FeeHeadId: 0,
                FeeAmount: 0
            };
            angular.forEach($scope.MonthFeeList, function (monthFee, monthFeeKey) {
                if (monthFee.IsChecked && monthFee.IsEnabled) {
                    angular.forEach(monthFee.FeeHeadDetails, function (feeHead, feeHeadKey) {
                        $scope.checkFeeHeadGroups(feeHead);
                    })
                    if (monthFee.LateFineFeeHead != null) {
                        if (lateFine.FeeHeadId > 0) {
                            monthFee.LateFineFeeHead.FeeAmount += monthFee.LateFineFeeHead.FeeAmount;
                        } else {
                            lateFine = monthFee.LateFineFeeHead;
                        }
                    }
                }
            })
            if (lateFine.FeeAmount != 0) {
                var newFeeHead = {
                    Concession: lateFine.Concession,
                    EnableEdit: lateFine.EnableEdit,
                    FeeAmount: lateFine.FeeAmount,
                    FeeHeadId: lateFine.FeeHeadId,
                    FeeHeadTitle: lateFine.FeeHeadTitle,
                    FeeType: lateFine.FeeType,
                    IsAdmissionFee: lateFine.IsAdmissionFee,
                    IsFine: lateFine.IsFine,
                    IsSelected: lateFine.IsSelected,
                    LineTotal: lateFine.LineTotal,
                    StudentChargeId: lateFine.StudentChargeId,
                }
                $scope.FeeHeadGroupList.push(newFeeHead);
            }
            angular.forEach($scope.StudentChargeFeeHeadList, function (feeHead, key) {
                $scope.checkFeeHeadGroups(feeHead);
            })

            //Calculate Due Fee Heads
            angular.forEach($scope.DueFeeHeadGroupList, function (dueFeeHead, feeHeadKey) {
                var IsAdded = false;
                for (var i = 0; i < $scope.FeeHeadGroupList.length; i++) {
                    var feeHeadGroup = $scope.FeeHeadGroupList[i];
                    if (dueFeeHead.FeeHeadId == feeHeadGroup.FeeHeadId && dueFeeHead.FeeType == feeHeadGroup.FeeType) {
                        feeHeadGroup.FeeAmount += dueFeeHead.FeeAmount;
                        feeHeadGroup.LineTotal += dueFeeHead.FeeAmount;
                        IsAdded = true;
                        break;
                    }
                }
                if (!IsAdded) {
                    if (dueFeeHead.FeeAmount != 0) {
                        var newFeeHead = {
                            Concession: dueFeeHead.Concession,
                            EnableEdit: dueFeeHead.EnableEdit,
                            FeeAmount: dueFeeHead.FeeAmount,
                            FeeHeadId: dueFeeHead.FeeHeadId,
                            FeeHeadTitle: dueFeeHead.FeeHeadTitle,
                            FeeType: dueFeeHead.FeeType,
                            IsAdmissionFee: dueFeeHead.IsAdmissionFee,
                            IsFine: dueFeeHead.IsFine,
                            IsSelected: dueFeeHead.IsSelected,
                            LineTotal: dueFeeHead.LineTotal,
                            StudentChargeId: dueFeeHead.StudentChargeId,
                        }
                        $scope.FeeHeadGroupList.push(newFeeHead);
                    }
                }

            });
            if (isReset) {
                $scope.calculateYearFeeHeadGroups(false);
            } else {
                $scope.calculateFeeHeadTotal();
            }
        }

        $scope.calculateFeeHeadTotal = function () {
            console.log($scope.FeeHeadGroupList);
            $scope.FeeCollection.LineTotal = 0;
            $scope.FeeCollection.Concession = 0;
            $scope.FeeCollection.NetLineTotal = 0;

            angular.forEach($scope.FeeHeadGroupList, function (feeHeadGroup, key) {
                if (feeHeadGroup.IsSelected && feeHeadGroup.FeeAmount > 0) {
                    $scope.FeeCollection.LineTotal += feeHeadGroup.FeeAmount;
                    $scope.FeeCollection.NetLineTotal += feeHeadGroup.LineTotal;
                    $scope.FeeCollection.Concession += feeHeadGroup.Concession;
                }
            })
            $scope.FeeCollection.Subtotal = $scope.FeeCollection.NetLineTotal + $scope.FeeCollection.PreviousDues;
            $scope.FeeCollection.PaidAmount = $scope.FeeCollection.Subtotal;
            $scope.FeeCollection.DuesAmount = 0;

        }

        $scope.calculateFeeHeadLineTotal = function (feeHeadGroup) {
            feeHeadGroup.LineTotal = feeHeadGroup.FeeAmount - feeHeadGroup.Concession;
            if (feeHeadGroup.LineTotal < 0) {
                feeHeadGroup.Concession = 0;
                feeHeadGroup.LineTotal = feeHeadGroup.FeeAmount - feeHeadGroup.Concession;
                toastr.error("Invalid Concession Amount.")
            }
            $scope.calculateFeeHeadTotal();
        }

        $scope.calculateDuesAmount = function () {
            $scope.FeeCollection.DuesAmount = $scope.FeeCollection.Subtotal - $scope.FeeCollection.PaidAmount;
            if ($scope.FeeCollection.DuesAmount < 0) {
                $scope.FeeCollection.PaidAmount = $scope.FeeCollection.Subtotal;
                $scope.FeeCollection.DuesAmount = 0;
                toastr.error("Payment amount should not be more than " + $scope.FeeCollection.Subtotal);
            }
        }

        //Due Fee Head Groups
        $scope.calculateDueFeeHeadGroups = function () {
            $scope.DueFeeHeadGroupList = [];
            //Due Fee List From Semester And Admission 
            angular.forEach($scope.MonthFeeList, function (monthFee, monthFeeKey) {
                if (!monthFee.IsEnabled) {
                    angular.forEach(monthFee.FeeHeadDetails, function (feeHead, feeHeadKey) {
                        var IsAdded = false;
                        for (var i = 0; i < $scope.DueFeeHeadGroupList.length; i++) {
                            var feeHeadGroup = $scope.DueFeeHeadGroupList[i];
                            if (feeHead.FeeHeadId == feeHeadGroup.FeeHeadId && feeHead.FeeType == feeHeadGroup.FeeType) {
                                feeHeadGroup.FeeAmount += feeHead.FeeAmount;
                                feeHeadGroup.LineTotal += feeHead.FeeAmount;
                                IsAdded = true;
                                break;
                            }
                        }
                        if (!IsAdded) {
                            if (feeHead.FeeAmount != 0) {
                                var newFeeHead = {
                                    Concession: feeHead.Concession,
                                    EnableEdit: feeHead.EnableEdit,
                                    FeeAmount: feeHead.FeeAmount,
                                    FeeHeadId: feeHead.FeeHeadId,
                                    FeeHeadTitle: feeHead.FeeHeadTitle,
                                    FeeType: feeHead.FeeType,
                                    IsAdmissionFee: feeHead.IsAdmissionFee,
                                    IsFine: feeHead.IsFine,
                                    IsSelected: feeHead.IsSelected,
                                    LineTotal: feeHead.LineTotal,
                                    StudentChargeId: feeHead.StudentChargeId,
                                }
                                $scope.DueFeeHeadGroupList.push(newFeeHead);
                            }
                        }
                    })
                }
            })

            //Due Fee List From Month
            angular.forEach($scope.YearFeeList, function (y1) {
                angular.forEach(y1.MonthFeeList, function (monthFee, monthFeeKey) {
                    if (!monthFee.IsEnabled) {
                        angular.forEach(monthFee.FeeHeadDetails, function (feeHead, feeHeadKey) {
                            var IsAdded = false;
                            for (var i = 0; i < $scope.DueFeeHeadGroupList.length; i++) {
                                var feeHeadGroup = $scope.DueFeeHeadGroupList[i];
                                if (feeHead.FeeHeadId == feeHeadGroup.FeeHeadId && feeHead.FeeType == feeHeadGroup.FeeType) {
                                    feeHeadGroup.FeeAmount += feeHead.FeeAmount;
                                    feeHeadGroup.LineTotal += feeHead.FeeAmount;
                                    IsAdded = true;
                                    break;
                                }
                            }
                            if (!IsAdded) {
                                if (feeHead.FeeAmount != 0) {
                                    var newFeeHead = {
                                        Concession: feeHead.Concession,
                                        EnableEdit: feeHead.EnableEdit,
                                        FeeAmount: feeHead.FeeAmount,
                                        FeeHeadId: feeHead.FeeHeadId,
                                        FeeHeadTitle: feeHead.FeeHeadTitle,
                                        FeeType: feeHead.FeeType,
                                        IsAdmissionFee: feeHead.IsAdmissionFee,
                                        IsFine: feeHead.IsFine,
                                        IsSelected: feeHead.IsSelected,
                                        LineTotal: feeHead.LineTotal,
                                        StudentChargeId: feeHead.StudentChargeId,
                                    }
                                    $scope.DueFeeHeadGroupList.push(newFeeHead);
                                }
                            }
                        })
                    }
                })
            })

            //Check Paid Fee in Due Fee List
            angular.forEach($scope.DueFeeHeadGroupList, function (dueFeeHead, key) {
                for (var i = 0; i < $scope.PaidFeeHeadList.length; i++) {
                    var paidFeeHead = $scope.PaidFeeHeadList[i];
                    if (dueFeeHead.FeeHeadId == paidFeeHead.FeeHeadId && dueFeeHead.FeeType == paidFeeHead.FeeType) {
                        dueFeeHead.FeeAmount = dueFeeHead.FeeAmount - paidFeeHead.FeeAmount;
                        dueFeeHead.LineTotal = dueFeeHead.LineTotal - paidFeeHead.LineTotal;
                        break;
                    }
                }
            })

            //Add Paid Fee Heads In Due List
            angular.forEach($scope.PaidFeeHeadList, function (paidFee, key) {
                var IsAdded = false;
                for (var i = 0; i < $scope.DueFeeHeadGroupList.length; i++) {
                    var dueFee = $scope.DueFeeHeadGroupList[i];
                    if (dueFee.FeeHeadId == paidFee.FeeHeadId && dueFee.FeeType == paidFee.FeeType) {
                        IsAdded = true;
                        break;
                    }
                }
                if (!IsAdded) {
                    if (paidFee.FeeAmount != 0) {
                        var newFeeHead = {
                            Concession: paidFee.Concession,
                            EnableEdit: paidFee.EnableEdit,
                            FeeAmount: -paidFee.FeeAmount,
                            FeeHeadId: paidFee.FeeHeadId,
                            FeeHeadTitle: paidFee.FeeHeadTitle,
                            FeeType: paidFee.FeeType,
                            IsAdmissionFee: paidFee.IsAdmissionFee,
                            IsFine: paidFee.IsFine,
                            IsSelected: paidFee.IsSelected,
                            LineTotal: -paidFee.LineTotal,
                            StudentChargeId: paidFee.StudentChargeId,
                        }
                        $scope.DueFeeHeadGroupList.push(newFeeHead);
                    }
                }
            })
            $scope.calculateFeeHeadGroups(true);
        }

        //Year Fee
        $scope.cahngeYearFee = function (yearIndex, monthIndex, monthFeeModel) {
            if (monthFeeModel.IsChecked) {
                for (var i = yearIndex; i >= 0; i--) {
                    var yearModel = $scope.YearFeeList[i];
                    if (yearModel.Year == monthFeeModel.Year) {
                        for (var j = monthIndex; j >= 0; j--) {
                            yearModel.MonthFeeList[j].IsChecked = true;
                        }
                    } else {
                        angular.forEach(yearModel.MonthFeeList, function (m1) {
                            m1.IsChecked = true;
                        })
                    }
                }
            }
            else {
                for (var i = yearIndex; i < $scope.YearFeeList.length; i++) {
                    var yearModel = $scope.YearFeeList[i];
                    if (yearModel.Year == monthFeeModel.Year) {
                        for (var j = monthIndex; j < yearModel.MonthFeeList.length; j++) {
                            yearModel.MonthFeeList[j].IsChecked = false;
                        }
                    } else {
                        angular.forEach(yearModel.MonthFeeList, function (m1) {
                            m1.IsChecked = false;
                        })
                    }
                }
            }
            $scope.calculateYearFeeHeadGroups(true);
        }

        $scope.calculateYearFeeHeadGroups = function (isReset) {
            if (isReset)
                $scope.FeeHeadGroupList = [];
            angular.forEach($scope.YearFeeList, function (y1) {
                angular.forEach(y1.MonthFeeList, function (monthFee) {
                    if (monthFee.IsChecked && monthFee.IsEnabled) {
                        angular.forEach(monthFee.FeeHeadDetails, function (feeHead) {
                            $scope.checkFeeHeadGroups(feeHead);
                        })
                        if (monthFee.LateFineFeeHead != null) {
                            if (lateFine.FeeHeadId > 0) {
                                monthFee.LateFineFeeHead.FeeAmount += monthFee.LateFineFeeHead.FeeAmount;
                            } else {
                                lateFine = monthFee.LateFineFeeHead;
                            }
                        }
                    }
                })
            })
            if (isReset) {
                $scope.calculateFeeHeadGroups(false);
            } else {
                $scope.calculateFeeHeadTotal();
            }
        }

        $scope.viewFeeHeadGroups = function () {
            $('#modal_fee_head_groups').modal('show');
        }

        $scope.saveFeePayment = function () {
            var saveAdmissionModel = {
                AdmissionId: $scope.SelectedAdmission.AdmissionId,
                MonthFeeHeadList: $scope.MonthFeeList,
                YearFeeList: $scope.YearFeeList,
                FeeHeadGroupList: $scope.FeeHeadGroupList,
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
                    RemarksForStudent: $scope.FeeCollection.RemarksForStudent,
                    ChequeNo: $scope.FeeCollection.ChequeNo,
                    ChequeDate: loadDataService.getDateTime($scope.FeeCollection.ChequeDate),
                    ChequeBank: $scope.FeeCollection.ChequeBank,
                    ChequeBankBranch: $scope.FeeCollection.ChequeBankBranch,
                    DemandDraftNo: $scope.FeeCollection.DemandDraftNo,
                    DemandDraftDate: loadDataService.getDateTime($scope.FeeCollection.DemandDraftDate),
                    DemandDraftBank: $scope.FeeCollection.DemandDraftBank,
                    TransactionNo: $scope.FeeCollection.TransactionNo,
                }

            }
            $scope.dataLoading = true;
            adminService.saveFeePayment(saveAdmissionModel)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        toastr.success("Payment Taken Successfully. Receipt No: " + response.data.ReceiptNo);
                        $scope.resetForm();
                        $('#modal_confirmation').modal('hide');
                        window.open("/admin/PrintCaseFeeReceipt/" + response.data.FeeCollectionId);
                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                }, function (err) {
                    toastr.error("Some error occured while fetching data");
                    $scope.dataLoading = false;
                })
        }

        $scope.saveFeePaymentConfirmation = function () {
            $scope.IsSubmitted = true;
            if (!$scope.IsFormValid) {
                toastr.error("Fill all required fields.")
                return;
            }
            if ($scope.FeeCollection.PaymentMode == 5) {
                if ($scope.FeeCollection.ChequeNo == null || $scope.FeeCollection.ChequeNo == undefined) {
                    toastr.error("Cheque No is required !!")
                    return;
                }
            }
            if ($scope.FeeCollection.PaidAmount > 0) {
                $scope.saveFeePayment();
            } else {
                $('#modal_confirmation').modal('show');
            }
        }

        //Student Remarks
        $scope.sort2 = function (key) {
            $scope.sortKey2 = key;
            $scope.reverse2 = !$scope.reverse2;
        };
        $scope.StudentRemark = {};
        $scope.StudentRemarkList = [];
        $scope.getStudentRemarkList = function (StudentId) {
            $scope.dataLoading = true;
            adminService.getStudentRemarkList(StudentId)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.StudentRemarkList = response.data.StudentRemarkList;
                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                }, function (err) {
                    toastr.error("Some error occured while fetching data");
                    $scope.dataLoading = false;
                })
        }

        $scope.saveStudentRemark = function () {
            if ($scope.SelectedAdmission.StudentId > 0) {
                if ($scope.StudentRemark.Remarks == undefined || $scope.StudentRemark.Remarks == null || $scope.StudentRemark.Remarks == "") {
                    toastr.error("Student remarks is required !");
                    return;
                }

                var StudentRemarkModel = {
                    StudentId: $scope.SelectedAdmission.StudentId,
                    Remarks: $scope.StudentRemark.Remarks,
                }

                $scope.dataLoading = true;
                adminService.saveStudentRemark(StudentRemarkModel)
                    .then(function (response) {
                        if (response.data.Message == 'Success') {
                            toastr.success("Remarks saved successfully.");
                            $scope.StudentRemarkList = response.data.StudentRemarkList;
                            $scope.StudentRemark = {};
                        } else {
                            toastr.error(response.data.Message);
                        }
                        $scope.dataLoading = false;
                    }, function (err) {
                        toastr.error("Some error occured while fetching data");
                        $scope.dataLoading = false;
                    })
            } else {
                toastr.error("Admission No is required!", "Validation Error");
            }
        }

        $scope.deleteStudentRemarkConfirmation = function (StudentRemarkId) {
            $scope.StudentRemarkId = StudentRemarkId;
            $('#modal_delete_confirmation').modal('show');
        }

        $scope.deleteStudentRemark = function () {
            $scope.dataLoading = true;
            adminService.deleteStudentRemark($scope.StudentRemarkId)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        toastr.success("Deleted Successfully.")
                        $scope.StudentRemarkList = response.data.StudentRemarkList;
                        $('#modal_delete_confirmation').modal('hide');
                        $scope.StudentRemarkId = null;
                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                }, function (err) {
                    toastr.error("Some error occured while fetching data");
                    $scope.dataLoading = false;
                })
        }
    }
})();
