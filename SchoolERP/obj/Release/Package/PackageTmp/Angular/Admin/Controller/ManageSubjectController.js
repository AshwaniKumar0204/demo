(function () {
    'use strict';

    angular
        .module('app')
        .controller('ManageSubjectController', ManageSubjectController);

    ManageSubjectController.$inject = ["$scope", "adminService", "ConstantData", "Factory"];

    function ManageSubjectController($scope, adminService, ConstantData, Factory) {
        $scope.checkSysytemLogin();
        $scope.currentPage = 1;
        $scope.sizeOptions = Factory.SizeOptions;
        $scope.pageSize = $scope.sizeOptions[1];
        $scope.sort = function (key) {
            $scope.sortKey = key;
            $scope.reverse = !$scope.reverse;
        };
        $scope.SubjectStatusList = ConstantData.SubjectStatus;

        $scope.Subject = {};
        $scope.Subject.SubjectStatus = null;

        $scope.calculateTotal = function () {
            $scope.ParentSubjectList = [];
            angular.forEach($scope.SubjectList, function (subject, key) {
                if (subject.ParentSubjectId == null)
                    $scope.ParentSubjectList.push(subject);
            })
        }

        $scope.getSubjectList = function () {
            $scope.dataLoading = true;
            adminService.getSubjectList(0)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.SubjectList = response.data.SubjectList;
                        $scope.calculateTotal();
                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                }, function (err) {
                    toastr.error("Some error occured while fetching data");
                    $scope.dataLoading = false;
                })
        }
        $scope.getSubjectList();

        $scope.getClassList = function () {
            $scope.dataLoading = true;
            adminService.getClassList(1)
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

        $scope.$watch('formSubject.$valid', function (value) {
            $scope.IsFormValid = value;
        })

        $scope.saveSubject = function () {
            $scope.IsSubmitted = true;
            if ($scope.IsFormValid) {
                var SubjectModel = {
                    SubjectId: $scope.Subject.SubjectId,
                    SubjectStatus: $scope.Subject.SubjectStatus,
                    SubjectName: $scope.Subject.SubjectName,
                    SubjectCode: $scope.Subject.SubjectCode,
                    ParentSubjectId: $scope.Subject.ParentSubjectId,
                }

                $scope.dataLoading = true;
                adminService.saveSubject(SubjectModel)
                    .then(function (response) {
                        if (response.data.Message == 'Success') {
                            if (SubjectModel.SubjectId > 0)
                                toastr.success("Subject Updated Successfully.");
                            else
                                toastr.success("New Subject Created Successfully.");
                            $scope.resetForm();
                            $scope.SubjectList = response.data.SubjectList;
                            $scope.calculateTotal();
                            $('#modal_Student_Charge').modal('hide');
                        } else {
                            toastr.error(response.data.Message);
                        }
                        $scope.dataLoading = false;
                    }, function (err) {
                        toastr.error("Some error occured while fetching data");
                        $scope.dataLoading = false;
                    })

            } else {
                toastr.error("Fill all fields!", "Validation Error");
            }
        }
        $scope.Subject = {};
        $scope.Subject.SubjectStatus = null;
        $scope.resetForm = function () {
            $scope.IsSubmitted = false;
            $scope.Subject.SubjectId = null;
            $scope.Subject.SubjectCode = "";
            $scope.Subject.SubjectName = "";
            $scope.formSubject.$setPristine();
            $scope.formSubject.$setUntouched();
        }

        $scope.newSubject = function () {
            $scope.resetForm();
            $('#modal_Student_Charge').modal('show');
        }

        $scope.editSubject = function (SubjectModel) {
            $scope.resetForm();
            //$scope.Subject = SubjectModel;
            $scope.Subject.SubjectName = SubjectModel.SubjectName;
            $scope.Subject.SubjectId = SubjectModel.SubjectId;
            $scope.Subject.SubjectStatus = SubjectModel.SubjectStatus;
            $scope.Subject.SubjectCode = SubjectModel.SubjectCode;
            $scope.Subject.ParentSubjectId = SubjectModel.ParentSubjectId;
            $('#modal_Student_Charge').modal('show');
        }

        $scope.deleteConfirmation = function (SubjectId) {
            $scope.SubjectId = SubjectId;
            $('#modal_confirmation').modal('show');
        }

        $scope.deleteSubject = function () {
            $scope.dataLoading = true;
            adminService.deleteSubject($scope.SubjectId)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        toastr.success("One Subject deleted successfully.", "Deleted Successfully.")
                        $scope.SubjectList = response.data.SubjectList;
                        $scope.SubjectId = null;
                        $('#modal_confirmation').modal('hide');
                        $scope.calculateTotal();
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
