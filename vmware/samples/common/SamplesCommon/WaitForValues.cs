/*
 * Copyright 2014, 2016 VMware, Inc.  All rights reserved.
 */

namespace vmware.samples.common
{
    using authentication;
    using System;
    using vmware.vim25;

    /// <summary>
    /// Helper for server side tasks.
    /// </summary>
    public class WaitForValues
    {
        private readonly VimAuthenticationHelper vimConnection;

        public WaitForValues(VimAuthenticationHelper vimConnection)
        {
            this.vimConnection = vimConnection;
        }

        /// <summary>
        /// Returns a boolean value specifying whether the task has succeeded
        /// or failed.
        /// </summary>
        /// <param name="task">
        /// ManagedObjectReference representing the task
        /// </param>
        /// <returns>boolean value representing the task result</returns>
        public bool GetTaskResultAfterDone(ManagedObjectReference task)
        {
            var retVal = false;

            // info has a property - state for state of the task
            var result = Wait(
                task,
                new string[] { "info.state", "info.error" },
                new string[] { "state" },
                new object[][] { new object[] {
                    TaskInfoState.success, TaskInfoState.error } }
                    );

            if (result[0].Equals(TaskInfoState.success))
            {
                retVal = true;
            }
            if (result[1] is LocalizedMethodFault)
            {
                throw new Exception(
                    ((LocalizedMethodFault)result[1]).localizedMessage);
            }
            return retVal;
        }

        /// <summary>
        /// Waits until any of the expected values of an object property has
        /// reached.
        /// </summary>
        /// <param name="task">ManagedObjectReference representing the task
        /// </param>
        /// <param name="filterProps">properties list to filter</param>
        /// <param name="endWaitProps">properties list to check for expected
        /// values, these are properties of a property in the filter properties
        /// list</param>
        /// <param name="expectedVals">expected values</param>
        /// <returns>task result</returns>
        public object[] Wait(ManagedObjectReference task,
            string[] filterProps,
            string[] endWaitProps,
            object[][] expectedVals)
        {
            // version is initially empty
            var version = "";
            var endVals = new object[endWaitProps.Length];
            var filterVals = new object[filterProps.Length];

            var propFilterSpec = PropertyFilterSpec(task, filterProps);
            var filterMoref = vimConnection.VimPortType.CreateFilter(
                vimConnection.ServiceContent.propertyCollector,
                propFilterSpec, true);

            var reached = false;
            UpdateSet updateset = null;
            while (!reached)
            {
                updateset = vimConnection.VimPortType.WaitForUpdatesEx(
                    vimConnection.ServiceContent.propertyCollector,
                    version,
                    new WaitOptions());

                if (updateset == null || updateset.filterSet == null)
                {
                    continue;
                }

                version = updateset.version;
                foreach (var filterUpdate in updateset.filterSet)
                {
                    foreach (var objUpdate in filterUpdate.objectSet)
                    {
                        // TODO: Handle all "kind"s of updates.
                        if (objUpdate.kind == ObjectUpdateKind.modify ||
                            objUpdate.kind == ObjectUpdateKind.enter ||
                            objUpdate.kind == ObjectUpdateKind.leave)
                        {
                            foreach (var propChange in objUpdate.changeSet)
                            {
                                UpdateValues(
                                    endWaitProps, endVals, propChange);
                                UpdateValues(
                                    filterProps, filterVals, propChange);
                            }
                        }
                    }
                }

                // check if we have reached the expected values
                for (var chgi = 0; chgi < endVals.Length && !reached; chgi++)
                {
                    for (var vali = 0;
                        vali < expectedVals[chgi].Length && !reached; vali++)
                    {
                        reached = expectedVals[chgi][vali].Equals(
                            endVals[chgi]);
                    }
                }
            }

            // destroy the filter when we are done
            vimConnection.VimPortType.DestroyPropertyFilter(filterMoref);
            return filterVals;
        }

        private PropertyFilterSpec PropertyFilterSpec(
            ManagedObjectReference objmor, string[] filterProps)
        {
            PropertyFilterSpec spec = new PropertyFilterSpec();
            ObjectSpec oSpec = new ObjectSpec();
            oSpec.obj = objmor;
            oSpec.skip = false;
            oSpec.skipSpecified = true;
            spec.objectSet = new ObjectSpec[] { oSpec };

            PropertySpec pSpec = new PropertySpec();
            pSpec.pathSet = filterProps;
            pSpec.type = objmor.type;
            spec.propSet = new PropertySpec[] { pSpec };

            return spec;
        }

        private void UpdateValues(string[] props, object[] vals,
            PropertyChange propchg)
        {
            for (int findi = 0; findi < props.Length; findi++)
            {
                if (propchg.name.LastIndexOf(props[findi]) >= 0)
                {
                    if (propchg.op == PropertyChangeOp.remove)
                        vals[findi] = "";
                    else
                        vals[findi] = propchg.val;
                }
            }
        }
    }
}
