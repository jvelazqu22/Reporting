using Domain.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace iBank.Services.Implementation.Shared.WhereRoute
{
    public class WhereRouteApplier<T> where T : class, IRouteWhere
    {
        public IList<T> GetDataBasedOnOriginCriteria(IList<T> data, IList<string> criteria, bool notInList, bool returnAllLegs)
        {
            if (notInList)
            {
                if (returnAllLegs)
                {
                    //if a leg w/in a trip has an origin that matches the filter don't return any of the legs in that trip
                    var reckeysOfMatchingLegs = data.Where(x => criteria.Contains(x.Origin?.Trim())).Select(x => x.RecKey).ToList();
                    return data.Where(x => !reckeysOfMatchingLegs.Contains(x.RecKey)).ToList();
                }
                else
                {
                    //if a leg w/in a trip has an origin that matches the filter don't return that leg but do return any other legs 
                    return data.Where(d => !criteria.Contains(d.Origin?.Trim())).ToList();
                }
            }
            else
            {
                if (returnAllLegs)
                {
                    //if a leg w/in a trip has an origin that matches the filter return all the legs in the trip
                    var reckeysOfMatchingLegs = data.Where(x => criteria.Contains(x.Origin?.Trim())).Select(x => x.RecKey).ToList();
                    return data.Where(x => reckeysOfMatchingLegs.Contains(x.RecKey)).ToList();
                }
                else
                {
                    //if a leg w/in a trip has an origin that matches the filter return that leg
                    return data.Where(x => criteria.Contains(x.Origin?.Trim())).ToList();
                }
            }
        }
        
        public IList<T> GetDataBasedOnOriginAndModeCriteria(IList<T> data, IList<string> criteria, bool notInList, bool returnAllLegs)
        {
            if (notInList)
            {
                if (returnAllLegs)
                {
                    //if a leg w/in a trip has an origin that matches the filter don't return any of the legs in that trip
                    var reckeysOfMatchingLegs = data.Where(x => criteria.Contains(x.Origin?.Trim().PadRight(10) + x.Mode))
                                                    .Select(x => x.RecKey).ToList();
                    return data.Where(x => !reckeysOfMatchingLegs.Contains(x.RecKey)).ToList();
                }
                else
                {
                    //if a leg w/in a trip has an origin that matches the filter don't return that leg but do return any other legs 
                    return data.Where(x => !criteria.Contains(x.Origin?.Trim().PadRight(10) + x.Mode)).ToList();
                }
            }
            else
            {
                if (returnAllLegs)
                {
                    //if a leg w/in a trip has an origin that matches the filter return all the legs in the trip
                    var reckeysOfMatchingLegs = data.Where(x => criteria.Contains(x.Origin?.Trim().PadRight(10) + x.Mode))
                                                    .Select(x => x.RecKey).ToList();
                    return data.Where(x => reckeysOfMatchingLegs.Contains(x.RecKey)).ToList();
                }
                else
                {
                    //if a leg w/in a trip has an origin that matches the filter return that leg
                    return data.Where(x => criteria.Contains(x.Origin?.Trim().PadRight(10) + x.Mode)).ToList();
                }
            }
        }
        
        public IList<T> GetDataBasedOnDestinationCriteria(IList<T> data, IList<string> criteria, bool notInList, bool returnAllLegs)
        {
            if (notInList)
            {
                if (returnAllLegs)
                {
                    //if a leg w/in a trip has an origin that matches the filter don't return any of the legs in that trip
                    var reckeysOfMatchingLegs = data.Where(x => criteria.Contains(x.Destinat?.Trim())).Select(x => x.RecKey).ToList();
                    return data.Where(x => !reckeysOfMatchingLegs.Contains(x.RecKey)).ToList();
                }
                else
                {
                    //if a leg w/in a trip has an origin that matches the filter don't return that leg but do return any other legs 
                    return data.Where(d => !criteria.Contains(d.Destinat?.Trim())).ToList();
                }
            }
            else
            {
                if (returnAllLegs)
                {
                    //if a leg w/in a trip has an origin that matches the filter return all the legs in the trip
                    var reckeysOfMatchingLegs = data.Where(x => criteria.Contains(x.Destinat?.Trim())).Select(x => x.RecKey).ToList();
                    return data.Where(x => reckeysOfMatchingLegs.Contains(x.RecKey)).ToList();
                }
                else
                {
                    //if a leg w/in a trip has an origin that matches the filter return that leg
                    return data.Where(x => criteria.Contains(x.Destinat?.Trim())).ToList();
                }
            }
        }
        
        public IList<T> GetDataBasedOnDestinationAndModeCriteria(IList<T> data, IList<string> criteria, bool notInList, bool returnAllLegs)
        {
            if (notInList)
            {
                if (returnAllLegs)
                {
                    //if a leg w/in a trip has an origin that matches the filter don't return any of the legs in that trip
                    var reckeysOfMatchingLegs = data.Where(x => criteria.Contains(x.Destinat?.Trim().PadRight(10) + x.Mode))
                                                    .Select(x => x.RecKey).ToList();
                    return data.Where(x => !reckeysOfMatchingLegs.Contains(x.RecKey)).ToList();
                }
                else
                {
                    //if a leg w/in a trip has an origin that matches the filter don't return that leg but do return any other legs 
                    return data.Where(x => !criteria.Contains(x.Destinat?.Trim().PadRight(10) + x.Mode)).ToList();
                }
            }
            else
            {
                if (returnAllLegs)
                {
                    //if a leg w/in a trip has an origin that matches the filter return all the legs in the trip
                    var reckeysOfMatchingLegs = data.Where(x => criteria.Contains(x.Destinat?.Trim().PadRight(10) + x.Mode))
                                                    .Select(x => x.RecKey).ToList();
                    return data.Where(x => reckeysOfMatchingLegs.Contains(x.RecKey)).ToList();
                }
                else
                {
                    //if a leg w/in a trip has an origin that matches the filter return that leg
                    return data.Where(x => criteria.Contains(x.Destinat?.Trim().PadRight(10) + x.Mode)).ToList();
                }
            }
        }
        
        public IList<T> GetDataBasedOnOriginAndDestinationCriteria(IList<T> data, IList<string> originCriteria, IList<string> destinationCriteria, bool notIn, bool returnAllLegs)
        {
            if (notIn)
            {
                if (returnAllLegs)
                {
                    var reckeysOfMatchingLegs = data.Where(x => (originCriteria.Contains(x.Origin?.Trim()) && destinationCriteria.Contains(x.Destinat?.Trim()))
                                                               || (originCriteria.Contains(x.Destinat?.Trim()) && destinationCriteria.Contains(x.Origin?.Trim())))
                                                    .Select(x => x.RecKey).ToList();
                    return data.Where(x => !reckeysOfMatchingLegs.Contains(x.RecKey)).ToList();
                }
                else
                {
                    //data that doesn't match origin nor destination criteria - bi directional
                    var originData = data.Where(x => !originCriteria.Contains(x.Origin?.Trim()) && !originCriteria.Contains(x.Destinat?.Trim())).ToList();
                    //data that doesn't match destination nor match origin criteria - bi directional
                    var destinationData = data.Where(x => !destinationCriteria.Contains(x.Destinat?.Trim()) && !destinationCriteria.Contains(x.Origin?.Trim())).ToList();

                    //now combine the two sets 
                    originData.AddRange(destinationData);
                    return originData.Distinct().ToList();
                }
            }
            else
            {
                if (returnAllLegs)
                {
                    var reckeysOfMatchingLegs = data.Where(x => originCriteria.Contains(x.Origin?.Trim()) && destinationCriteria.Contains(x.Destinat?.Trim())
                                                            || (originCriteria.Contains(x.Destinat?.Trim()) && destinationCriteria.Contains(x.Origin?.Trim())))
                                                    .Select(x => x.RecKey).ToList();

                    //traced from top caller, this is for bidicrectional so it should only get legs that either origin or destination are in these places
                    return data.Where(x => reckeysOfMatchingLegs.Contains(x.RecKey)).ToList();
                }
                else
                {
                    //bi directional
                    return data.Where(x => (originCriteria.Contains(x.Origin?.Trim()) && destinationCriteria.Contains(x.Destinat?.Trim()))
                                           || (originCriteria.Contains(x.Destinat?.Trim()) && destinationCriteria.Contains(x.Origin?.Trim()))).ToList();
                }
            }
        }

        public IList<T> GetDataBasedOnOriginAndDestinationPlusModeCriteria(IList<T> data, IList<string> originCriteria, IList<string> destinationCriteria,
                                                                           bool notIn, bool returnAllLegs)
        {
            if (notIn)
            {
                if (returnAllLegs)
                {
                    var reckeysOfMatchingLegs = data.Where(x => (originCriteria.Contains(x.Origin?.Trim().PadRight(10) + x.Mode) && destinationCriteria.Contains(x.Destinat?.Trim().PadRight(10) + x.Mode))
                                                               || (originCriteria.Contains(x.Destinat?.Trim().PadRight(10) + x.Mode) && destinationCriteria.Contains(x.Origin?.Trim().PadRight(10) + x.Mode)))
                                                    .Select(x => x.RecKey).ToList();
                    return data.Where(x => !reckeysOfMatchingLegs.Contains(x.RecKey)).ToList();
                }
                else
                {
                    //data that doesn't match origin nor destination criteria - bi directional
                    var originData = data.Where(x => !originCriteria.Contains(x.Origin?.Trim().PadRight(10) + x.Mode) && !originCriteria.Contains(x.Destinat?.Trim().PadRight(10) + x.Mode)).ToList();
                    //data that doesn't match destination nor match origin criteria - bi directional
                    var destinationData = data.Where(x => !destinationCriteria.Contains(x.Destinat?.Trim().PadRight(10) + x.Mode) && !destinationCriteria.Contains(x.Origin?.Trim().PadRight(10) + x.Mode)).ToList();

                    //now combine the two sets 
                    originData.AddRange(destinationData);
                    return originData.Distinct().ToList();
                }
            }
            else
            {
                if (returnAllLegs)
                {
                    var reckeysOfMatchingLegs = data.Where(x => originCriteria.Contains(x.Origin?.Trim().PadRight(10) + x.Mode) && destinationCriteria.Contains(x.Destinat?.Trim().PadRight(10) + x.Mode)
                                                            || (originCriteria.Contains(x.Destinat?.Trim().PadRight(10) + x.Mode) && destinationCriteria.Contains(x.Origin?.Trim().PadRight(10) + x.Mode)))
                                                    .Select(x => x.RecKey).ToList();

                    //traced from top caller, this is for bidicrectional so it should only get legs that either origin or destination are in these places
                    return data.Where(x => reckeysOfMatchingLegs.Contains(x.RecKey)).ToList();
                }
                else
                {
                    //bi directional
                    return data.Where(x => (originCriteria.Contains(x.Origin?.Trim().PadRight(10) + x.Mode) && destinationCriteria.Contains(x.Destinat?.Trim().PadRight(10) + x.Mode))
                                           || (originCriteria.Contains(x.Destinat?.Trim().PadRight(10) + x.Mode) && destinationCriteria.Contains(x.Origin?.Trim().PadRight(10) + x.Mode))).ToList();
                }
            }
        }

        /// <summary>
        /// Examines both the origin or destination against the supplied criteria.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="criteria"></param>
        /// <param name="notIn"></param>
        /// <param name="returnAllLegs"></param>
        /// <returns></returns>
        public IList<T> GetDataBasedOnOriginOrDestinationCriteria(IList<T> data, IList<string> criteria, bool notIn, bool returnAllLegs)
        {
            if (notIn)
            {
                if (returnAllLegs)
                {
                    var reckeysOfMatchingLegs = data.Where(x => (criteria.Contains(x.Destinat?.Trim()) || criteria.Contains(x.Origin?.Trim()))
                                                        && (criteria.Contains(x.Origin?.Trim()) || criteria.Contains(x.Destinat?.Trim())))
                                                    .Select(x => x.RecKey).ToList();
                    return data.Where(x => !reckeysOfMatchingLegs.Contains(x.RecKey)).ToList();
                }
                else
                {
                    //data that doesn't match origin criteria and doesn't match destinat
                    return data.Where(x => !criteria.Contains(x.Origin?.Trim()) && !criteria.Contains(x.Destinat?.Trim())).ToList();
                }
            }
            else
            {
                if (returnAllLegs)
                {
                    var reckeysOfMatchingLegs = data.Where(x => criteria.Contains(x.Destinat?.Trim()) || criteria.Contains(x.Origin?.Trim()))
                                                    .Select(x => x.RecKey).ToList();
                    return data.Where(x => reckeysOfMatchingLegs.Contains(x.RecKey)).ToList();
                }
                else
                {
                    return data.Where(x => criteria.Contains(x.Destinat?.Trim()) || criteria.Contains(x.Origin?.Trim())).ToList();
                }
            }
        }
        
        public IList<T> GetDataBasedOnOriginOrDestinationPlusModeCriteria(IList<T> data, IList<string> criteria, bool notIn, bool returnAllLegs)
        {
            if (notIn)
            {
                if (returnAllLegs)
                {
                    var reckeysOfMatchingLegs = data.Where(x => (criteria.Contains(x.Destinat?.Trim().PadRight(10) + x.Mode) || criteria.Contains(x.Origin?.Trim().PadRight(10) + x.Mode))
                                                        && (criteria.Contains(x.Origin?.Trim().PadRight(10) + x.Mode) || criteria.Contains(x.Destinat?.Trim().PadRight(10) + x.Mode)))
                                                    .Select(x => x.RecKey).ToList();
                    return data.Where(x => !reckeysOfMatchingLegs.Contains(x.RecKey)).ToList();
                }
                else
                {
                    //data that doesn't match origin criteria and doesn't match destinat
                    return data.Where(x => !criteria.Contains(x.Origin?.Trim().PadRight(10) + x.Mode) && !criteria.Contains(x.Destinat?.Trim().PadRight(10) + x.Mode)).ToList();
                }
            }
            else
            {
                if (returnAllLegs)
                {
                    var reckeysOfMatchingLegs = data.Where(x => criteria.Contains(x.Destinat?.Trim().PadRight(10) + x.Mode) || criteria.Contains(x.Origin?.Trim().PadRight(10) + x.Mode))
                                                    .Select(x => x.RecKey).ToList();
                    return data.Where(x => reckeysOfMatchingLegs.Contains(x.RecKey)).ToList();
                }
                else
                {
                    return data.Where(x => criteria.Contains(x.Destinat?.Trim().PadRight(10) + x.Mode) || criteria.Contains(x.Origin?.Trim().PadRight(10) + x.Mode)).ToList();
                }
            }
        }
    }
}
