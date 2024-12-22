(function () {
    'use strict';

    angular
        .module('app')
        .controller('ManageParentController', ManageParentController);

    ManageParentController.$inject = ["$scope", "adminService", "ConstantData", "Factory"];

    function ManageParentController($scope, adminService, ConstantData, Factory) {
        $scope.checkSysytemLogin();
        $scope.currentPage = 1;
        $scope.sizeOptions = Factory.SizeOptions;
        $scope.pageSize = $scope.sizeOptions[0];
        $scope.sort = function (key) {
            $scope.sortKey = key;
            $scope.reverse = !$scope.reverse;
        };
        $scope.ParentStatusList = ConstantData.ParentStatus;

        $scope.getSearchStudentList = function () {
            $scope.dataLoading = true;
            adminService.getSearchStudentList(0)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.StudentList = response.data.StudentList;
                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                }, function (err) {
                    toastr.error("Some error occured while fetching data");
                    $scope.dataLoading = false;
                })
        }
        $scope.getSearchStudentList()

        $scope.afterStudentSelected = function (obj) {
            if (obj != null) {
                $scope.Student = obj.originalObject;
            } else {
                $scope.Student = {};
            }
        }

        $scope.addStudent = function () {
            if ($scope.Student.StudentId == null || $scope.Student.StudentId == undefined) {
                toastr.error("Student is required !!");
                return;
            }
            for (var i = 0; i < $scope.Parent.StudentList.length; i++) {
                var Student = $scope.Parent.StudentList[i];
                if ($scope.Student.StudentId == Student.StudentId) {
                    toastr.error("This Student is already added.");
                    return;
                }
            }
            var Student = {
                StudentId: $scope.Student.StudentId,
                AdmissionNo: $scope.Student.AdmissionNo,
                FullName: $scope.Student.FullName,
            }
            $scope.Parent.StudentList.push(Student);
            $scope.Student = {};
            $scope.$broadcast('angucomplete-alt:clearInput', StudentAnguComplete)
            document.getElementById('StudentAnguComplete_value').value = '';
        }

        $scope.deleteStudent = function (index) {
            $scope.Parent.StudentList.splice(index, 1);
        }

        $scope.getParentList = function () {
            $scope.dataLoading = true;
            adminService.getParentList(0)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.ParentList = response.data.ParentList;
                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                }, function (err) {
                    toastr.error("Some error occured while fetching data");
                    $scope.dataLoading = false;
                })
        }
        $scope.getParentList();

        $scope.$watch('formParent.$valid', function (value) {
            $scope.IsFormValid = value;
        })

        $scope.saveParent = function () {
            $scope.IsSubmitted = true;
            if ($scope.IsFormValid) {
                var ParentModel = {
                    ParentId: $scope.Parent.ParentId,
                    ParentStatus: $scope.Parent.ParentStatus,
                    ParentCode: $scope.Parent.ParentCode,
                    LoginPassword: $scope.Parent.LoginPassword,
                    FatherMobileNo: $scope.Parent.FatherMobileNo,
                    MotherMobileNo: $scope.Parent.MotherMobileNo,
                    StudentList: $scope.Parent.StudentList
                }

                $scope.dataLoading = true;
                adminService.saveParent(ParentModel)
                    .then(function (response) {
                        if (response.data.Message == 'Success') {
                            if (ParentModel.ParentId > 0)
                                toastr.success("Parent Updated Successfully.");
                            else
                                toastr.success("New Parent Created Successfully.");
                            $scope.resetForm();
                            $('#modal_Student_Charge').modal('hide');
                            $scope.getParentList();
                        } else {
                            toastr.error(response.data.Message);
                            $scope.dataLoading = false;
                        }
                    }, function (err) {
                        toastr.error("Some error occured while fetching data");
                        $scope.dataLoading = false;
                    })

            } else {
                toastr.error("Fill all fields!", "Validation Error");
            }
        }

        $scope.resetForm = function () {
            $scope.IsSubmitted = false;
            $scope.Parent = {};
            $scope.Parent.ParentStatus = null;
            $scope.Parent.StudentList = [];
            $scope.formParent.$setPristine();
            $scope.formParent.$setUntouched();
        }

        $scope.newParent = function () {
            $scope.resetForm();
            $('#modal_Student_Charge').modal('show');
        }

        $scope.editParent = function (ParentModel) {
            $scope.resetForm();
            $scope.Parent = ParentModel;
            $scope.Parent.FatherMobileNo = parseInt(ParentModel.FatherMobileNo);
            $scope.Parent.MotherMobileNo = parseInt(ParentModel.MotherMobileNo);
            $('#modal_Student_Charge').modal('show');
        }

        $scope.deleteConfirmation = function (ParentId) {
            $scope.ParentId = ParentId;
            $('#modal_confirmation').modal('show');
        }

        $scope.deleteParent = function () {
            $scope.dataLoading = true;
            adminService.deleteParent($scope.ParentId)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        toastr.success("One Parent deleted successfully.", "Deleted Successfully.")
                        $scope.ParentId = null;
                        $scope.getParentList();
                        $('#modal_confirmation').modal('hide');
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
