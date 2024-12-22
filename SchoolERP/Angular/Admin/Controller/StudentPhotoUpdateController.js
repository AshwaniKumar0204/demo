(function () {
    'use strict';

    angular
        .module('app')
        .controller('StudentPhotoUpdateController', StudentPhotoUpdateController);

    StudentPhotoUpdateController.$inject = ['$scope', 'adminService'];

    function StudentPhotoUpdateController($scope, adminService) {
        $scope.checkSysytemLogin();
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

        $scope.getAdmissionDetail = function (AdmissionId, InputChange) {
            $scope.dataLoading = true;
            adminService.getAdmissionDetail(AdmissionId)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.Admission = response.data.AdmissionDetail;
                        $scope.AdmissionId = $scope.Admission.AdmissionId;
                        if (InputChange)
                            $scope.$broadcast('angucomplete-alt:changeInput', 'StudentAutoComplete', $scope.Admission.Student.AdmissionNo + " (" + $scope.Admission.Student.FullName + ")");
                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                }, function (err) {
                    toastr.error("Some error occured while fetching data");
                    $scope.dataLoading = false;
                })
        }

        $scope.checkSession = function () {
            var AdmisssionId = sessionStorage.getItem('admissionId');
            if (AdmisssionId > 0) {
                $scope.getAdmissionDetail(AdmisssionId, true);
                sessionStorage.removeItem('admissionId');
            } else {
                $scope.getSearchAdmissionList();
            }
        }
        $scope.checkSession();

        $scope.afterStudentSelected = function (selected) {
            if (selected != undefined) {
                $scope.SelectedAdmission = selected.originalObject;
                if ($scope.AdmissionId != $scope.SelectedAdmission.AdmissionId) {
                    $scope.getAdmissionDetail($scope.SelectedAdmission.AdmissionId, false);
                }
            };
        }

        $scope.checkStudentPhoto = function (element) {
            if (element.files[0].type != 'image/jpeg' && element.files[0].type != 'image/jpg') {
                document.getElementById("studentPhoto").value = "";
                toastr.error("File Format should be in jpg format.");
            } else if (element.files[0].size > 302400) {
                document.getElementById("studentPhoto").value = "";
                toastr.error("File size should be less than 300 KB.");
            }
        }

        $scope.uploadStudentPhoto = function () {
            var photo = document.getElementById("studentPhoto").value;
            if (photo == "") {
                toastr.error("Validation Error", "Selected photo is invalid !");
                return;
            }
            var formData = new FormData();
            formData.append('AdmissionId', $scope.Admission.AdmissionId);
            formData.append('StudentPhoto', document.getElementById("studentPhoto").files[0]);
            $scope.dataLoading = true;
            adminService.uploadStudentPhoto(formData)
                .then(function (response) {
                    if (response.data.Message == "Success") {
                        $scope.Admission = response.data.AdmissionDetail;
                        toastr.success("Success", "Photo Uploaded Successfully.");
                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                }, function (err) {
                    toastr.error("Some error occured while fething data.");
                    $scope.dataLoading = false;
                })
        }
    }
})();
