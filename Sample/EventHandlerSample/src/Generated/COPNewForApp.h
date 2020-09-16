#ifndef __RTCOP_GENERATED_COPNEWFORAPP__
#define __RTCOP_GENERATED_COPNEWFORAPP__

#include "RTCOP/COPNew.h"

namespace baselayer { class Sample; } 
namespace RTCOP {
namespace Generated {

template<class Base>
inline const int GetBaseClassID() { return -1; }
template<>
inline const int GetBaseClassID<baselayer::Sample>() { return 0; }

} // namespace Generated {}

template<class Base, class... ArgTypes>
inline Core::LayerdObject<Base>* copnew(ArgTypes... args)
{
	const int classId = Generated::GetBaseClassID<Base>();
return COPNew<Base>(classId, args...);
}

} // namespace RTCOP {}
#endif // !__RTCOP_GENERATED__COPNEW__
