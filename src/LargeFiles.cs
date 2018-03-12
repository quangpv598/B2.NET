﻿using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using B2Net.Http;
using B2Net.Http.RequestGenerators;
using B2Net.Models;
using Newtonsoft.Json;

namespace B2Net {
	public class LargeFiles {
		private B2Options _options;
		private HttpClient _client;

		public LargeFiles(B2Options options) {
			_options = options;
			_client = HttpClientFactory.CreateHttpClient(options.RequestTimeout);
		}

        /// <summary>
        /// Starts a large file upload.
        /// </summary>
        /// <param name="fileId"></param>
        /// <param name="fileName"></param>
        /// <param name="bucketId"></param>
        /// <param name="cancelToken"></param>
        /// <returns></returns>
        public async Task<B2File> StartLargeFile(string fileName, string contentType = "", string bucketId = "", Dictionary<string, string> fileInfo = null, CancellationToken cancelToken = default(CancellationToken)) {
            var operationalBucketId = Utilities.DetermineBucketId(_options, bucketId);

            var request = LargeFileRequestGenerators.Start(_options, operationalBucketId, fileName, contentType, fileInfo);

            // Send the download request
            var response = await _client.SendAsync(request, cancelToken);

            // Create B2File from response
            return await ResponseParser.ParseResponse<B2File>(response);
        }

        /// <summary>
        /// Get an upload url for use with one Thread.
        /// </summary>
        /// <param name="bucketId"></param>
        /// <param name="cancelToken"></param>
        /// <returns></returns>
        public async Task<B2UploadPartUrl> GetUploadPartUrl(string fileId, CancellationToken cancelToken = default(CancellationToken)) {
            var request = LargeFileRequestGenerators.GetUploadPartUrl(_options, fileId);

            var uploadUrlResponse = await _client.SendAsync(request, cancelToken);

            var uploadUrl = await ResponseParser.ParseResponse<B2UploadPartUrl>(uploadUrlResponse);

            return uploadUrl;
        }

        /// <summary>
        /// Upload one part of an already started large file upload.
        /// </summary>
        /// <param name="fileData"></param>
        /// <param name="fileName"></param>
        /// <param name="bucketId"></param>
        /// <param name="cancelToken"></param>
        /// <returns></returns>
        public async Task<B2UploadPart> UploadPart(byte[] fileData, int partNumber, B2UploadPartUrl uploadPartUrl, CancellationToken cancelToken = default(CancellationToken)) {
            var request = LargeFileRequestGenerators.Upload(_options, fileData, partNumber, uploadPartUrl);

            var response = _client.SendAsync(request, cancelToken).Result;
            
            return await ResponseParser.ParseResponse<B2UploadPart>(response);
        }

	    /// <summary>
	    /// Downloads one file by providing the name of the bucket and the name of the file.
	    /// </summary>
	    /// <param name="fileId"></param>
	    /// <param name="fileName"></param>
	    /// <param name="bucketId"></param>
	    /// <param name="cancelToken"></param>
	    /// <returns></returns>
	    public async Task<B2File> FinishLargeFile(string fileId, string[] partSHA1Array, CancellationToken cancelToken = default(CancellationToken)) {
	        var request = LargeFileRequestGenerators.Finish(_options, fileId, partSHA1Array);

	        // Send the request
	        var response = await _client.SendAsync(request, cancelToken);

	        // Create B2File from response
	        return await ResponseParser.ParseResponse<B2File>(response);
        }

        /// <summary>
        /// List the parts of an incomplete large file upload.
        /// </summary>
        /// <param name="fileId"></param>
        /// <param name="startPartNumber"></param>
        /// <param name="maxPartCount"></param>
        /// <param name="cancelToken"></param>
        /// <returns></returns>
	    public async Task<B2LargeFileParts> ListPartsForIncompleteFile(string fileId, int startPartNumber, int maxPartCount, CancellationToken cancelToken = default(CancellationToken)) {
	        var request = LargeFileRequestGenerators.ListParts(_options, fileId, startPartNumber, maxPartCount);

	        // Send the request
	        var response = await _client.SendAsync(request, cancelToken);

	        // Create B2File from response
	        return await ResponseParser.ParseResponse<B2LargeFileParts>(response);
	    }
        
        /// <summary>
        /// Cancel a large file upload
        /// </summary>
        /// <param name="fileId"></param>
        /// <param name="cancelToken"></param>
        /// <returns></returns>
	    public async Task<B2CancelledFile> CancelLargeFile(string fileId, CancellationToken cancelToken = default(CancellationToken)) {
	        var request = LargeFileRequestGenerators.Cancel(_options, fileId);

	        // Send the request
	        var response = await _client.SendAsync(request, cancelToken);

	        // Create B2File from response
	        return await ResponseParser.ParseResponse<B2CancelledFile>(response);
	    }

        /// <summary>
        /// List all the incomplete large file uploads for the supplied bucket
        /// </summary>
        /// <param name="bucketId"></param>
        /// <param name="startFileId"></param>
        /// <param name="maxFileCount"></param>
        /// <param name="cancelToken"></param>
        /// <returns></returns>
	    public async Task<B2IncompleteLargeFiles> ListIncompleteFiles(string bucketId, string startFileId = "", string maxFileCount = "", CancellationToken cancelToken = default(CancellationToken)) {
	        var request = LargeFileRequestGenerators.IncompleteFiles(_options, bucketId, startFileId, maxFileCount);

	        // Send the request
	        var response = await _client.SendAsync(request, cancelToken);

	        // Create B2File from response
	        return await ResponseParser.ParseResponse<B2IncompleteLargeFiles>(response);
	    }
    }
}