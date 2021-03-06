﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;

namespace SinjulMSBH_Upload_Web.Controllers
{
	public class UploaderController: Controller
	{
		private IHostingEnvironment hostingEnvironment;

		public UploaderController ( IHostingEnvironment hostingEnvironment )
		{
			this.hostingEnvironment = hostingEnvironment;
		}

		public IActionResult Upload01 ( ) => View( );

		[HttpPost]
		public async Task<IActionResult> Index ( IList<IFormFile> files )
		{
			Startup.Progress = 0;

			long totalBytes = files.Sum(f => f.Length);

			foreach ( IFormFile source in files )
			{
				string filename = ContentDispositionHeaderValue.Parse(source.ContentDisposition).FileName.ToString().Trim('"');

				filename = this.EnsureCorrectFilename( filename );

				byte[] buffer = new byte[16 * 1024];

				using ( FileStream output = System.IO.File.Create( this.GetPathAndFilename( filename ) ) )
				{
					using ( Stream input = source.OpenReadStream( ) )
					{
						long totalReadBytes = 0;
						int readBytes;

						while ( ( readBytes = input.Read( buffer , 0 , buffer.Length ) ) > 0 )
						{
							await output.WriteAsync( buffer , 0 , readBytes );
							totalReadBytes += readBytes;
							Startup.Progress = ( int ) ( ( float ) totalReadBytes / ( float ) totalBytes * 100.0 );
							await Task.Delay( 10 ); // It is only to make the process slower
						}
					}
				}
			}

			return this.Content( "success" );
		}

		[HttpPost]
		public ActionResult Progress ( )
		{
			return this.Content( Startup.Progress.ToString( ) );
		}

		private string EnsureCorrectFilename ( string filename )
		{
			if ( filename.Contains( "\\" ) )
				filename = filename.Substring( filename.LastIndexOf( "\\" ) + 1 );

			return filename;
		}

		private string GetPathAndFilename ( string filename )
		{
			string path = this.hostingEnvironment.WebRootPath + "\\uploads\\";

			if ( !Directory.Exists( path ) )
				Directory.CreateDirectory( path );

			return path + filename;
		}

		public IActionResult Upload02 ( ) => View( );

		public IActionResult IFrame ( ) => View( );

		public IActionResult Upload03 ( ) => View( );

		public IActionResult Uploaded ( ) => View( );

		[HttpPost]
		public async Task<IActionResult> Index2 ( IList<IFormFile> files )
		{
			foreach ( IFormFile source in files )
			{
				string filename = ContentDispositionHeaderValue.Parse(source.ContentDisposition).FileName.ToString().Trim('"');

				filename = this.EnsureCorrectFilename( filename );

				using ( FileStream output = System.IO.File.Create( this.GetPathAndFilename( filename ) ) )
					await source.CopyToAsync( output );
			}

			return this.View( nameof( Uploaded ) );
		}

		public IActionResult Upload04 ( ) => View( );

		[HttpPost]
		public async Task<JsonResult> UploadFile ( )
		{
			try
			{
				foreach ( IFormFile source in Request.Form.Files )
				{
					string filename = ContentDispositionHeaderValue.Parse(source.ContentDisposition).FileName.ToString().Trim('"');

					filename = this.EnsureCorrectFilename( filename );

					using ( FileStream output = System.IO.File.Create( this.GetPathAndFilename( filename ) ) )
						await source.CopyToAsync( output );
				}
			}
			catch ( Exception )
			{
				Response.StatusCode = ( int ) HttpStatusCode.BadRequest;
				return Json( "Upload failed" );
			}

			return Json( "File uploaded successfully" );
		}

		//Upload Large File

		//public JsonResult UploadFile ( IList<IFormFile> files )
		//{
		//	return Json( new { state = 0 , message = string.Empty } );
		//}

		//[HttpPost]
		//public IActionResult Upload ( List<IFormFile> files )
		//{
		//	//Do something with the files here.
		//	return Ok( );
		//}

		//[HttpPost]
		//[DisableFormValueModelBinding]
		//public async Task<IActionResult> Index ( )
		//{
		//	FormValueProvider formModel;
		//	using ( var stream = System.IO.File.Create( "c:\\temp\\myfile.temp" ) )
		//	{
		//		formModel = await Request.StreamFile( stream );
		//	}

		//	var viewModel = new MyViewModel();

		//	var bindingSuccessful = await TryUpdateModelAsync(viewModel, prefix: "",valueProvider: formModel);

		//	if ( !bindingSuccessful )
		//	{
		//		if ( !ModelState.IsValid )
		//		{
		//			return BadRequest( ModelState );
		//		}
		//	}

		//	return Ok( viewModel );
		//}

		//public class MyViewModel
		//{
		//	public string Username { get; set; }
		//}
	}
}