﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Web;
using XStatic.Generator.Storage;
using XStatic.Generator.Transformers;

namespace XStatic.Generator
{
    public class Job
    {
        public int StaticSiteId { get; set; }

        public IFileNameGenerator NameGenerator { get; set; }

        public List<ITransformer> Transformers { get; set; } = new List<ITransformer>();

        public List<int> PageIds { get; set; } = new List<int>();

        public List<int> MediaIds { get; set; } = new List<int>();

        public List<string> Files { get; set; } = new List<string>();
    }

    public class JobRunner
    {
        private readonly IGenerator _generator;

        public JobRunner(IGenerator generator)
        {
            _generator = generator;
        }

        public async Task<IEnumerable<string>> RunJob(Job job)
        {
            var returnList = new List<string>();

            foreach (var id in job.PageIds)
            {
                returnList.Add(await _generator.Generate(id, job.StaticSiteId, job.NameGenerator, job.Transformers));
            }

            return returnList;
        }
    }

    public class JobBuilder
    {
        private Job job;

        public JobBuilder(int staticSiteId, IFileNameGenerator nameGenerator)
        {
            job = new Job
            {
                StaticSiteId = staticSiteId,
                NameGenerator = nameGenerator
            };
        }

        public JobBuilder AddTransformer(ITransformer transformer)
        {
            job.Transformers.Add(transformer);

            return this;
        }

        public JobBuilder AddPage(IPublishedContent node)
        {
            job.PageIds.Add(node.Id);

            return this;
        }

        public JobBuilder AddPageWithDescendants(IPublishedContent node)
        {
            job.PageIds.Add(node.Id);

            var childIds = node.Descendants().Select(c => c.Id);

            job.PageIds.AddRange(childIds);

            return this;
        }

        public Job Build()
        {
            return job;
        }
    }
}