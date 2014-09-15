package metis.client.sdk.utility;

/**
 * Created by Administrator on 14-9-5.
 */
public class Arguments {

    public static void notNullOrEmpty(String value, String parameterName) {
        if(null == value) {
            throw new NullPointerException(parameterName);
        }
        if("".equals(value) || value.isEmpty()) {
            throw new IllegalArgumentException(String.format(ArgumentStrings.Argument_EmptyArray,
                    parameterName));
        }
    }

    public static void notNull(Object value) {
        if(null == value) {
            throw new NullPointerException("value");
        }
    }

    private static class ArgumentStrings {
        public static final String Argument_EmptyArray =
                "'%s' must contain at least one element.";

        public static final String Argument_EmptyString =
                "'%s' cannot be an empty string or start with the null character.";

        public static final String Argument_NullElement =
                "'%s' cannot contain a null element.";

        public static final String Argument_Whitespace =
                "The parameter '%s' cannot consist entirely of white space characters.";
    }
}
