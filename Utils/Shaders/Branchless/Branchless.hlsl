#pragma once

#define DEFINE_BRANCHLESS_GT(NAME, PRECISION, VALTYPE)\
    void NAME(in PRECISION val, in PRECISION edge, in VALTYPE gtVal, in VALTYPE lteVal, out VALTYPE result){\
    PRECISION stepVal = step(edge, val);\
    result = stepVal * gtVal + (1.0 - stepVal) * lteVal;\
}

DEFINE_BRANCHLESS_GT(Branchless_GreaterThan_float, float, float)
DEFINE_BRANCHLESS_GT(Branchless_GreaterThan_half, half, half)

#define DEFINE_BRANCHLESS_BETWEEN(NAME, PRECISION, VALTYPE)\
    void NAME(in PRECISION val, in PRECISION lowerBound, in PRECISION upperBound, in VALTYPE insideVal, in VALTYPE outsideVal, out VALTYPE result){\
    PRECISION stepVal = step(lowerBound, val) * (1.0 - step(upperBound, val));\
    result = stepVal * insideVal + (1.0 - stepVal) * outsideVal;\
}

DEFINE_BRANCHLESS_BETWEEN(Branchless_Between_float, float, float)
DEFINE_BRANCHLESS_BETWEEN(Branchless_Between_half, half, half)