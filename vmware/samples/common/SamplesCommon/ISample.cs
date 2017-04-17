/*
 * Copyright 2013, 2016 VMware, Inc.  All rights reserved.
 * VMware Confidential.
 */

namespace vmware.samples.common
{
    /// <summary>
    /// Samples common interface.
    /// </summary>
    public interface ISample
    {
        /// <summary>
        /// Runs a specific sample. Samples are required to validate user
        /// input and provide usage help message.
        /// </summary>
        /// <param name="args">argument(s) to the sample.</param>
        void RunSample(params string[] args);
    }
}
