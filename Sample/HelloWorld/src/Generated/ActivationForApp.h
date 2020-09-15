#ifndef __RTCOP_GENERATED_ACTIVATIONFORAPP__
#define __RTCOP_GENERATED_ACTIVATIONFORAPP__

#include "RTCOP/Activation.h"

namespace RTCOP {
namespace Generated {

enum class LayerID : int
{
	baselayer = 0,
	English = 1,	Japanese = 2,};
} // namespace Generated {}

inline void activate(Generated::LayerID layerid)
{
	Activate((int)layerid);
}

inline void deactivate(Generated::LayerID layerid)
{
	Deactivate((int)layerid);
}

} // namespace RTCOP {}
#endif // !__RTCOP_GENERATED_ACTIVATION__
