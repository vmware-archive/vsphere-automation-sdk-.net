/**
 * *******************************************************
 * Copyright VMware, Inc. 2013, 2016.  All Rights Reserved.
 * SPDX-License-Identifier: MIT
 * *******************************************************
 *
 * DISCLAIMER. THIS PROGRAM IS PROVIDED TO YOU "AS IS" WITHOUT
 * WARRANTIES OR CONDITIONS OF ANY KIND, WHETHER ORAL OR WRITTEN,
 * EXPRESS OR IMPLIED. THE AUTHOR SPECIFICALLY DISCLAIMS ANY IMPLIED
 * WARRANTIES OR CONDITIONS OF MERCHANTABILITY, SATISFACTORY QUALITY,
 * NON-INFRINGEMENT AND FITNESS FOR A PARTICULAR PURPOSE.
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
