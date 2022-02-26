// Copyright (C) 2022 NeoAxis, Inc. Delaware, USA; NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;

namespace NeoAxis
{
	public class SearchJson
	{
		public class Cursors
		{
			public string next { get; set; }
			public object previous { get; set; }
		}

		public class Tag
		{
			public string name { get; set; }
			public string slug { get; set; }
			public string uri { get; set; }
		}

		public class Category
		{
			public string name { get; set; }
		}

		public class Image
		{
			public string uid { get; set; }
			public int size { get; set; }
			public int width { get; set; }
			public string url { get; set; }
			public int height { get; set; }
		}

		public class Thumbnails
		{
			public List<Image> images { get; set; }
		}

		public class Avatar
		{
			public string uri { get; set; }
			public List<Image> images { get; set; }
		}

		public class User
		{
			public string uid { get; set; }
			public string username { get; set; }
			public string displayName { get; set; }
			public string profileUrl { get; set; }
			public string account { get; set; }
			public Avatar avatar { get; set; }
			public string uri { get; set; }
		}

		public class Gltf
		{
			public int size { get; set; }
		}

		public class Archives
		{
			public Gltf gltf { get; set; }
		}

		public class Result
		{
			public string uri { get; set; }
			public string uid { get; set; }
			public string name { get; set; }
			//public DateTime staffpickedAt { get; set; }
			public int viewCount { get; set; }
			public int likeCount { get; set; }
			public int animationCount { get; set; }
			public string viewerUrl { get; set; }
			public string embedUrl { get; set; }
			public int commentCount { get; set; }
			public bool isDownloadable { get; set; }
			public DateTime publishedAt { get; set; }
			public List<Tag> tags { get; set; }
			public List<Category> categories { get; set; }
			public Thumbnails thumbnails { get; set; }
			public User user { get; set; }
			public string description { get; set; }
			public int faceCount { get; set; }
			public DateTime createdAt { get; set; }
			public int vertexCount { get; set; }
			public bool isAgeRestricted { get; set; }
			public Archives archives { get; set; }
		}

		public class Root
		{
			public Cursors cursors { get; set; }
			public string next { get; set; }
			public object previous { get; set; }
			public List<Result> results { get; set; }
		}
	}
}
