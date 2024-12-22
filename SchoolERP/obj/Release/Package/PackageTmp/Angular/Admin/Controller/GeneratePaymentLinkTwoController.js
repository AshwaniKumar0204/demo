(function () {
    'use strict';

    angular
        .module('app')
        .controller('GeneratePaymentLinkTwoController', GeneratePaymentLinkTwoController);

    GeneratePaymentLinkTwoController.$inject = ['$scope', 'adminService', 'Factory', 'ConstantData', 'loadDataService'];

    function GeneratePaymentLinkTwoController($scope, adminService, Factory, ConstantData, loadDataService) {
        $scope.checkSysytemLogin();
        $scope.currentPage = 1;
        $scope.sizeOptions = Factory.SizeOptions;
        $scope.pageSize = $scope.sizeOptions[0];
        $scope.sort = function (key) {
            $scope.sortKey = key;
            $scope.reverse = !$scope.reverse;
        };


        $scope.getClassStreamTypeList = function () {
            $scope.SubjectOptionOne = null;
            $scope.SubjectOptionTwo = null;
            $scope.ClassStreamTypeId = null;
            $scope.dataLoading = true;
            adminService.getClassStreamTypeList(1, $scope.ClassNo)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.ClassStreamTypeList = response.data.ClassStreamTypeList;
                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                }, function (err) {
                    toastr.error("Some error occured while fetching data");
                    $scope.dataLoading = false;
                })
        }
        $scope.ClassNo = 11;
        $scope.getClassStreamTypeList();
        
        $scope.getSubjectOptionList = function () {
            if ($scope.ClassStreamTypeId == null || $scope.ClassStreamTypeId == undefined) {
                return;
            }

            $scope.dataLoading = true;
            adminService.getSubjectOptionList($scope.ClassNo, $scope.ClassStreamTypeId, 0)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.SubjectOptionOneList = response.data.SubjectOptionList;
                        //$scope.changeSubjectOptionOne();
                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                }, function (err) {
                    toastr.error("Some error occured while fetching data");
                    $scope.dataLoading = false;
                })
            //$scope.changeSubjectOptionOne();
        }

        $scope.getAcademicSessionList = function () {
            $scope.dataLoading = true;
            adminService.getAcademicSessionList(ConstantData.SessionStatus[1].Key)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.AcademicSessionList = response.data.AcademicSessionList;
                        $scope.AcademicSessionId = response.data.AcademicSessionList[0].AcademicSessionId;
                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                }, function (err) {
                    toastr.error("Some error occured while fetching data");
                    $scope.dataLoading = false;
                })
        }
        $scope.getAcademicSessionList();

        $scope.getClassList = function () {
            $scope.dataLoading = true;
            adminService.getClassListDistinct(1)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.ClassList = response.data.ClassList;
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

        $scope.getBoardMarksStatusList = function () {
            $scope.dataLoading = true;
            adminService.getBoardMarksStatusList(ConstantData.SessionStatus[1].Key)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.BoardMarksStatusList = response.data.BoardMarksStatusList;
                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                }, function (err) {
                    toastr.error("Some error occured while fetching data");
                    $scope.dataLoading = false;
                })
        }
        $scope.getBoardMarksStatusList();

        $scope.getRegistrationFormList = function () {
            $scope.dataLoading = true;
            adminService.getSearchRegistrationFormList(0, $scope.AcademicSessionId, $scope.ClassNo)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.RegistrationFormList = response.data.RegistrationFormList;
                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                }, function (err) {
                    toastr.error("Some error occured while fetching data");
                    $scope.dataLoading = false;
                })
        }
        $scope.getRegistrationFormList();

        $scope.changeRegistrationForm = function () {
            $scope.SelectedRegistrationForm = {};
            for (var i = 0; i < $scope.RegistrationFormList.length; i++) {
                if ($scope.RegistrationFormId == $scope.RegistrationFormList[i].RegistrationFormId) {
                    $scope.SelectedRegistrationForm = $scope.RegistrationFormList[i];
                    break;
                }
            }
        }

        $scope.getResultTypeList = function () {
            $scope.dataLoading = true;
            adminService.getResultTypeList()
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.ResultTypeList = response.data.ResultTypeList;
                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                }, function (err) {
                    toastr.error("Some error occured while fetching data");
                    $scope.dataLoading = false;
                })
        }
        $scope.getResultTypeList();


        $scope.$watch('formUnit.$valid', function (value) {
            $scope.IsFormValid = value;
        })
        $scope.getRegistrationOnlineList = function () {
            $scope.IsSubmitted = true;
            if (!$scope.IsFormValid) {
                toastr.error("Fill all required fields!");
                return;
            }
            //if (!$scope.IsSameSchool)
            //    $scope.ResultType = 1;
            var obj = {
                AcademicSessionId: $scope.AcademicSessionId,
                ClassNo: $scope.ClassNo,
                RegistrationFormId: $scope.RegistrationFormId,
                ClassStreamTypeId: $scope.ClassStreamTypeId,
                SubjectOptionOne: $scope.SubjectOptionOne,
                SubjectOptionTwo: $scope.SubjectOptionTwo,
                IsSameSchool: $scope.IsSameSchool,
                ResultType: $scope.ResultType,
                AdmissionType: $scope.AdmissionType,
                BoardMarksStatus: $scope.BoardMarksStatus,
            }
            $scope.dataLoading = true;
            adminService.getRegistrationOnlineListForLinkTwo(obj)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        var RegistrationOnlineList = response.data.RegistrationOnlineList;
                        $scope.RegistrationOnlineList = [];
                        angular.forEach(RegistrationOnlineList, function (r1) {
                            r1.TotalFeeAmount = 0;
                            r1.TotalPaidAmount = 0;
                            r1.IsChecked = false;
                            angular.forEach(r1.PaidFeeList, function (p1) {
                                angular.forEach(r1.FeeList, function (f1) {
                                    if (p1.FeeHeadId == f1.FeeHeadId && p1.FeeType == f1.FeeType) {
                                        f1.Amount -= p1.Amount;
                                        r1.TotalPaidAmount += f1.Amount;
                                    }
                                })
                            })

                            angular.forEach(r1.PendingFeeList, function (p1) {
                                angular.forEach(r1.FeeList, function (f1) {
                                    if (p1.FeeHeadId == f1.FeeHeadId && p1.FeeType == f1.FeeType) {
                                        f1.Amount -= p1.Amount;
                                        r1.TotalPaidAmount += f1.Amount;
                                    }
                                })
                            })
                            angular.forEach(r1.FeeList, function (f1) {
                                r1.TotalFeeAmount += f1.Amount;
                            })
                            if (r1.TotalFeeAmount > 0)
                                $scope.RegistrationOnlineList.push(r1);
                        });
                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                }, function (err) {
                    toastr.error("Some error occured while fetching data");
                    $scope.dataLoading = false;
                })
        }

        $scope.calculateTotal = function (r1) {
            r1.TotalFeeAmount = 0;
            angular.forEach(r1.FeeList, function (f1) {
                r1.TotalFeeAmount += f1.Amount;
            })
        }

        $scope.viewFee = function (r1) {
            $scope.SelectedRegistrationOnline = r1;
            $('#modal_popUp').modal('show');
        }
        $scope.CheckedAll = function () {
            var IsCheckedAll = document.getElementById('IsCheckedAll').checked;
            angular.forEach($scope.RegistrationOnlineList, function (registrationOnline, key1) {
                if (IsCheckedAll)
                    registrationOnline.IsChecked = true;
                else
                    registrationOnline.IsChecked = false;
            })
        }
        $scope.changeChecked = function (registrationOnline) {
            registrationOnline.IsChecked = !registrationOnline.IsChecked;
        }

        $scope.previewPaymentLink = function () {
            $scope.RegistrationOnlineFeeList = [];
            angular.forEach($scope.RegistrationOnlineList, function (registrationOnline, key1) {
                if (registrationOnline.IsChecked) {
                    var obj = {
                        RegistrationOnlineId: registrationOnline.RegistrationOnlineId,
                        RegistrationNo: registrationOnline.RegistrationNo,
                        StudentName: registrationOnline.StudentName,
                        FathersName: registrationOnline.FathersName,
                        IsChecked: registrationOnline.IsChecked,
                        TotalFeeAmount: registrationOnline.TotalFeeAmount,
                        FeeList: registrationOnline.FeeList,
                    }
                    $scope.RegistrationOnlineFeeList.push(obj);
                }
            });
            $('#modal_preview').modal('show');
        }

        $scope.savePaymentLinkTwo = function () {
            var RegistrationList = [];
            angular.forEach($scope.RegistrationOnlineFeeList, function (registration, key1) {
                if (registration.IsChecked)
                    RegistrationList.push(registration);
            })

            if (RegistrationList.length == 0) {
                toastr.error("No Student is Selected");
                return;
            }

            var obj = {
                RegistrationOnlineList: RegistrationList,
                ExpiredDate: loadDataService.getDateTime($scope.ExpiredDate),
            }
            $scope.dataLoading = true;
            adminService.savePaymentLinkTwo(obj)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        toastr.success("Link Generated Successfully.");
                        $scope.RegistrationOnlineList = [];
                        $scope.ExpiredDate = null;
                        $('#modal_preview').modal('hide');
                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                }, function (err) {
                    toastr.error("Some error occured while fetching data");
                    $scope.dataLoading = false;
                })
        }

        $scope.tablesToExcel = function (tables, wsnames, wbname, appname) {
            var initailPage = $scope.pageSize;
            $scope.pageSize = $scope.sizeOptions[$scope.sizeOptions.length - 1];
            $scope.IsExporting = true;
            setTimeout(function () {
                tablesToExcel(tables, wsnames, wbname, appname);
            }, 500)
            setTimeout(function () {
                $scope.IsExporting = false;
                $scope.pageSize = initailPage;
                $scope.$apply();
            }, 1000)
        }
    }
})();
