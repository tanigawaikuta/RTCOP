#ifndef __RTCOP_GENERATED_COPNEWFORAPP__
#define __RTCOP_GENERATED_COPNEWFORAPP__

#include "RTCOP/COPNew.h"

namespace baselayer { class A; } 
namespace baselayer { class B; } 
namespace RTCOP {
namespace Generated {

template<class Base>
inline const int GetBaseClassID() { return -1; }
template<>
inline const int GetBaseClassID<baselayer::A>() { return 0; }
template<>
inline const int GetBaseClassID<baselayer::B>() { return 1; }

} // namespace Generated {}

template<class Base, class... ArgTypes>
inline Core::LayerdObject<Base>* copnew(ArgTypes... args)
{
	const int classId = Generated::GetBaseClassID<Base>();
	return COPNew<Base>(classId, args...);
}

} // namespace RTCOP {}
#endif // !__RTCOP_GENERATED__COPNEW__
