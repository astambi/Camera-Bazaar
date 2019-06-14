namespace CameraBazaar.Web.Controllers
{
    using System.Linq;
    using AutoMapper;
    using CameraBazaar.Data.Models;
    using CameraBazaar.Services;
    using CameraBazaar.Web.Models.Cameras;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;

    [Authorize]
    public class CamerasController : Controller
    {
        private const string CameraFormView = "CameraForm";

        private readonly ICameraService cameraService;
        private readonly UserManager<User> userManager;
        private readonly IMapper mapper;

        public CamerasController(
            ICameraService cameraService,
            UserManager<User> userManager,
            IMapper mapper)
        {
            this.cameraService = cameraService;
            this.userManager = userManager;
            this.mapper = mapper;
        }

        [AllowAnonymous]
        public IActionResult All()
        {
            var cameras = this.cameraService.All();
            return this.View(cameras);
        }

        [AllowAnonymous]
        public IActionResult Details(int id)
        {
            var camera = this.cameraService.GetByIdDetails(id);
            if (camera == null)
            {
                return this.RedirectToAction(nameof(All));
            }

            // Current user is owner => allow CRUD
            var currentUserId = this.userManager.GetUserId(this.User);
            if (camera.UserId == currentUserId)
            {
                camera.HasCrud = true;
            }

            return this.View(camera);
        }

        public IActionResult Create()
            => this.View(CameraFormView, new CameraFormViewModel());

        [HttpPost]
        public IActionResult Create(CameraFormViewModel cameraModel)
        {
            if (!cameraModel.LightMeteringSelectList.Any())
            {
                this.ModelState.AddModelError(nameof(cameraModel.LightMetering), "The Light metering is required.");
            }

            if (!this.ModelState.IsValid)
            {
                return this.View(CameraFormView, cameraModel);
            }

            this.cameraService.Create(
                cameraModel.Make,
                cameraModel.Model,
                cameraModel.Price,
                cameraModel.Quantity,
                cameraModel.MinShutterSpeed,
                cameraModel.MaxShutterSpeed,
                cameraModel.MinIso,
                cameraModel.MaxIso,
                cameraModel.IsFullFrame,
                cameraModel.VideoResolution,
                cameraModel.LightMeteringSelectList,
                cameraModel.Description,
                cameraModel.ImageUrl,
                this.userManager.GetUserId(this.User));

            return this.RedirectToAction(nameof(All));
        }

        public IActionResult Edit(int id)
        {
            // Camera does not exist
            if (!this.cameraService.Exists(id))
            {
                return this.RedirectToAction(nameof(All));
            }

            // Current user is not camera owner
            var currentUserId = this.userManager.GetUserId(this.User);
            if (currentUserId == null
                || !this.cameraService.ExistsWithOwner(id, currentUserId))
            {
                return this.RedirectToAction(nameof(Details), new { id });
            }

            var cameraModel = this.LoadCameraFormModel(id, nameof(Edit));

            return this.View(CameraFormView, cameraModel);
        }

        [HttpPost]
        public IActionResult Edit(int id, CameraFormViewModel cameraModel)
        {
            // Camera does not exist
            if (!this.cameraService.Exists(id))
            {
                return this.RedirectToAction(nameof(All));
            }

            // Current user is not camera owner
            var currentUserId = this.userManager.GetUserId(this.User);
            if (currentUserId == null
                || !this.cameraService.ExistsWithOwner(id, currentUserId))
            {
                return this.RedirectToAction(nameof(Details), new { id });
            }

            if (!cameraModel.LightMeteringSelectList.Any())
            {
                this.ModelState.AddModelError(nameof(cameraModel.LightMetering), "The Light metering is required.");
            }

            // Model is not valid
            if (!this.ModelState.IsValid)
            {
                return this.View(CameraFormView, cameraModel);
            }

            this.cameraService.Update(
                id,
                cameraModel.Make,
                cameraModel.Model,
                cameraModel.Price,
                cameraModel.Quantity,
                cameraModel.MinShutterSpeed,
                cameraModel.MaxShutterSpeed,
                cameraModel.MinIso,
                cameraModel.MaxIso,
                cameraModel.IsFullFrame,
                cameraModel.VideoResolution,
                cameraModel.LightMeteringSelectList,
                cameraModel.Description,
                cameraModel.ImageUrl,
                currentUserId);

            return this.RedirectToAction(nameof(Details), new { id });
        }

        public IActionResult Delete(int id)
        {
            // Camera does not exist
            if (!this.cameraService.Exists(id))
            {
                return this.RedirectToAction(nameof(All));
            }

            // Current user is not camera owner
            var currentUserId = this.userManager.GetUserId(this.User);
            if (currentUserId == null
                || !this.cameraService.ExistsWithOwner(id, currentUserId))
            {
                return this.RedirectToAction(nameof(Details), new { id });
            }

            var cameraModel = this.LoadCameraFormModel(id, nameof(Delete));


            return this.View(CameraFormView, cameraModel);
        }

        [HttpPost]
        public IActionResult Delete(int id, CameraFormViewModel cameraModel)
        {
            // Camera does not exist
            if (!this.cameraService.Exists(id))
            {
                return this.RedirectToAction(nameof(All));
            }

            // Current user is not camera owner
            var currentUserId = this.userManager.GetUserId(this.User);
            if (currentUserId == null
                || !this.cameraService.ExistsWithOwner(id, currentUserId))
            {
                return this.RedirectToAction(nameof(Details), new { id });
            }

            this.cameraService.Remove(id, currentUserId);

            return this.RedirectToAction(nameof(All));
        }

        private CameraFormViewModel LoadCameraFormModel(int id, string action)
        {
            var camera = this.cameraService.GetById(id);
            var cameraModel = this.mapper.Map<CameraFormViewModel>(camera);
            cameraModel.Action = action;

            return cameraModel;
        }
    }
}