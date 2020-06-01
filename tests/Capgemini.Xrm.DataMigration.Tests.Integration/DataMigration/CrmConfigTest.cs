﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Capgemini.Xrm.DataMigration.Config;
using Capgemini.Xrm.DataMigration.CrmStore.Config;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Capgemini.Xrm.DataMigration.IntegrationTests
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    [TestClass]
    public class CrmConfigTest
    {
        private const string ImportConfigExample = @"TestData/ImportConfigExample.json";

        [TestInitialize]
        public void Setup()
        {
            if (File.Exists(ImportConfigExample))
            {
                File.Delete(ImportConfigExample);
            }
        }

        [TestMethod]
        public void CrmImportProcessorConfigSaveTest()
        {
            CrmImportConfig mgr = new CrmImportConfig
            {
                MigrationConfig = new MappingConfiguration()
            };

            var guidMap = new Dictionary<Guid, Guid>
            {
                { Guid.Parse("4ee93159-38c1-e611-80e9-c4346bacbdc4"), Guid.Empty },
                { Guid.Parse("e47e726c-24bd-e611-80e6-c4346bad4198"), Guid.Empty },
                { Guid.Parse("75c28436-3ec1-e611-80e9-c4346bacbdc4"), Guid.Empty }
            };

            mgr.MigrationConfig.Mappings.Add("test1", guidMap);

            var guidMap2 = new Dictionary<Guid, Guid>
            {
                { Guid.Parse("018b25db-37c1-e611-80e9-c4346bacbdc4"), Guid.Empty },
                { Guid.Parse("e8a524d4-37c1-e611-80e9-c4346bacbdc4"), Guid.Empty },
                { Guid.Parse("c11b9c7b-3ac1-e611-80e9-c4346bacbdc4"), Guid.Empty },
                { Guid.Parse("c8144a2d-50bd-e611-80e6-c4346bad4198"), Guid.Empty }
            };
            mgr.MigrationConfig.Mappings.Add("test2", guidMap2);

            mgr.AdditionalFieldsToIgnore.AddRange(new List<string> { "TEST1", "Test2" });
            mgr.EntitiesToSync.AddRange(new List<string> { "TESTEnt1", "TestEnt2" });

            mgr.JsonFolderPath = @"C:\xxxx\xxx.json";

            mgr.PluginsToDeactivate.AddRange(new List<Tuple<string, string>> { new Tuple<string, string>("plugin name", "assembly nme"), new Tuple<string, string>("plugin name2", "assembly nme2") });
            mgr.ProcessesToDeactivate.AddRange(new List<string> { "test", "test2" });

            mgr.IgnoreStatuses = true;
            mgr.IgnoreStatusesExceptions.AddRange(new List<string>() { "ent1", "ent2" });

            mgr.SaveConfiguration(ImportConfigExample);

            Assert.IsTrue(File.Exists(ImportConfigExample));
        }

        [TestMethod]
        public void CrmImportProcessorConfigReadTest()
        {
            var mgr = CrmImportConfig.GetConfiguration(@"TestData/ImportConfig.json");

            Assert.IsNotNull(mgr);
        }

        [TestMethod]
        public void CrmExporterConfigSaveTest()
        {
            var exportConfig = $@"TestData/TempExportConfig.json";
            DeleteFileIfItExists(exportConfig);

            string folderPath = new FileInfo(Assembly.GetExecutingAssembly().Location).DirectoryName;
            string fetchXMLFolderPath = Path.Combine(folderPath, "ImportSchemas\\TestDataSchema");
            CrmExporterConfig config = new CrmExporterConfig() { FetchXMLFolderPath = fetchXMLFolderPath };

            config.CrmMigrationToolSchemaFilters.Add("entity1", "filter1");
            config.CrmMigrationToolSchemaFilters.Add("entity2", "filter2");
            config.CrmMigrationToolSchemaFilters.Add("entity3", "filter3");

            config.SaveConfiguration(exportConfig);

            Assert.IsTrue(File.Exists(exportConfig));
        }

        [TestMethod]
        public void CrmExporterConfigReadTest()
        {
            var mgr = CrmExporterConfig.GetConfiguration(@"TestData/ExportConfig.json");

            Assert.AreEqual("C:\\GitRepos\\UserSettings\\usersettingsschema.xml", mgr.CrmMigrationToolSchemaPaths[0]);
        }

        [TestMethod]
        public void CrmExporterConfigGetFetchXMLQueries1Test()
        {
            string folderPath = new FileInfo(Assembly.GetExecutingAssembly().Location).DirectoryName;
            string fetchXMLFolderPath = Path.Combine(folderPath, "TestData\\ImportSchemas\\TestDataSchema");
            CrmExporterConfig config = new CrmExporterConfig() { FetchXMLFolderPath = fetchXMLFolderPath };

            List<string> fetchXmls = config.GetFetchXMLQueries();
            Assert.IsTrue(fetchXmls.Count > 0);
        }

        [TestMethod]
        public void CrmExporterConfigGetFetchXMLQueries2Test()
        {
            string folderPath = new FileInfo(Assembly.GetExecutingAssembly().Location).DirectoryName;
            string configPath = Path.Combine(folderPath, "TestData\\ImportSchemas\\TestDataSchema\\usersettingsschema.xml");
            CrmExporterConfig config = new CrmExporterConfig();
            config.CrmMigrationToolSchemaPaths.Add(configPath);

            List<string> fetchXmls = config.GetFetchXMLQueries();
            Assert.IsTrue(fetchXmls.Count > 0);
        }

        [TestMethod]
        public void CrmSchemaConfigReadTest()
        {
            string folderPath = new FileInfo(Assembly.GetExecutingAssembly().Location).DirectoryName;
            string configPath = Path.Combine(folderPath, "TestData\\ImportSchemas\\TestDataSchema\\usersettingsschema.xml");

            CrmSchemaConfiguration config = CrmSchemaConfiguration.ReadFromFile(configPath);

            Assert.IsNotNull(config);
        }

        private static void DeleteFileIfItExists(string exportConfig)
        {
            if (File.Exists(exportConfig))
            {
                File.Delete(exportConfig);
            }
        }
    }
}