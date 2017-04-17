/*
 * Copyright 2014, 2016 VMware, Inc.  All rights reserved.
 */

namespace vmware.samples.tagging.workflow
{
    using CommandLine;
    using System;
    using System.Collections.Generic;
    using vapi.std;
    using vcenter.helpers;
    using vmware.cis.tagging;
    using vmware.samples.common;

    /// <summary>
    /// Demonstrates CRUD operations on a sample tag.
    /// </summary>
    public class TaggingWorkflow : SamplesBase
    {
        Tag tagService;
        Category categoryService;
        TagAssociation tagAssociation;
        string tagId, categoryId;
        string tagName, categoryName;
        DynamicID clusterId;
        private Boolean tagAttached;

        [Option(
            "cluster",
            HelpText = "Name of a Cluster to be tagged.",
            Required = true)]
        public string ClusterName { get; set; }

        [Option(
            "datacenter",
            HelpText = "Name of the Datacenter on which to create tags.",
            Required = true)]
        public string DatacenterName { get; set; }
        public override void Run()
        {
            // Get the cluster
            this.clusterId = new DynamicID();
            this.clusterId.SetType("ClusterComputeResource");
            this.clusterId.SetId(ClusterHelper.GetCluster(
                VapiAuthHelper.StubFactory, SessionStubConfiguration,
                DatacenterName, ClusterName));

            this.tagName = RandomIdGenerator.GetRandomString("Tag-");
            var tagDesc = "Sample tag";
            this.categoryName = RandomIdGenerator.GetRandomString("Cat-");
            var categoryDesc = "Sample category";

            // create services
            this.tagService = VapiAuthHelper.StubFactory.CreateStub<Tag>(
                SessionStubConfiguration);
            this.categoryService =
                VapiAuthHelper.StubFactory.CreateStub<Category>(
                    SessionStubConfiguration);
            this.tagAssociation =
                VapiAuthHelper.StubFactory.CreateStub<TagAssociation>(
                    SessionStubConfiguration);

            // create a category
            this.categoryId = CreateCategory(categoryService, categoryName,
                categoryDesc, CategoryModel.Cardinality.MULTIPLE);
            Console.WriteLine("Created category '{0}'", categoryName);

            // create a tag
            this.tagId = CreateTag(
                this.tagService, this.tagName, tagDesc, this.categoryId);
            Console.WriteLine("Created tag '{0}'", this.tagName);

            // update the category
            var newCategoryDesc = "Tag category updated at " + DateTime.Now;
            UpdateCategoryDesc(this.categoryService, this.categoryId,
                newCategoryDesc);
            Console.WriteLine("Updated category description to '{0}'",
                newCategoryDesc);

            // update the tag
            var newTagDesc = "Tag updated at " + DateTime.Now;
            UpdateTagDesc(tagService, tagId, newTagDesc);
            Console.WriteLine("Updated tag description to '{0}'", newTagDesc);

            // tag the Cluster with the newely created tag
            this.tagAssociation.Attach(this.tagId, this.clusterId);
            if (this.tagAssociation.ListAttachedTags(
                this.clusterId).Contains(this.tagId))
            {
                Console.WriteLine("Cluster '{0}' tagged with '{1}'",
                    ClusterName, tagName);
                this.tagAttached = true;
            }
            else
            {
                throw new Exception(string.Format(
                    "Could not tag Cluster '{0}' with '{1}'",
                    ClusterName, tagName));
            }
        }

        public override void Cleanup()
        {
            // detach the Tag from the Cluster
            if (this.tagAttached)
            {
                this.tagAssociation.Detach(tagId, this.clusterId);
                Console.WriteLine("Cluster '{0}' untagged", ClusterName);
            }

            // delete the tag
            if (this.tagId != null)
            {
                this.tagService.Delete(this.tagId);
                Console.WriteLine("Deleted tag '{0}'", this.tagName);
            }


            // delete the category
            if(this.categoryId != null)
            {
                this.categoryService.Delete(categoryId);
                Console.WriteLine("Deleted category '{0}'", this.categoryName);
            }
        }

        private string CreateCategory(
            Category categoryService,
            string name,
            string description,
            CategoryModel.Cardinality cardinality)
        {
            var createSpec = new CategoryTypes.CreateSpec();
            createSpec.SetName(name);
            createSpec.SetDescription(description);
            createSpec.SetCardinality(cardinality);
            createSpec.SetAssociableTypes(new HashSet<String>());
            return categoryService.Create(createSpec);
        }

        private void UpdateCategoryDesc(
            Category categoryService,
            string categoryId,
            string description)
        {
            var category = categoryService.Get(categoryId);
            Console.WriteLine("Read category '{0}'", category.GetName());
            var updateSpec = new CategoryTypes.UpdateSpec();
            updateSpec.SetDescription(description);
            categoryService.Update(categoryId, updateSpec);
        }

        private string CreateTag(
            Tag taggingService,
            string name,
            string description,
            string categoryId)
        {
            var createSpec = new TagTypes.CreateSpec();
            createSpec.SetName(name);
            createSpec.SetDescription(description);
            createSpec.SetCategoryId(categoryId);
            return taggingService.Create(createSpec);
        }

        private void UpdateTagDesc(
            Tag taggingService,
            string tagId,
            string description)
        {
            var tag = taggingService.Get(tagId);
            Console.WriteLine("Read tag '{0}'", tag.GetName());
            var updateSpec = new TagTypes.UpdateSpec();
            updateSpec.SetDescription(description);
            taggingService.Update(tagId, updateSpec);
        }

        public static void Main(string[] args)
        {
            new TaggingWorkflow().Execute(args);
        }
    }
}
